﻿using Entities.DataTransferObject;
using Entities.Models;
using Entities.RequestFeatures;
using Microsoft.AspNetCore.JsonPatch;
using Microsoft.AspNetCore.Mvc;
using Presentation.ActionFilters;
using Services.Contracts;
using System.Text.Json;
using static Entities.Exceptions.NotFound;

namespace Presentation.Controllers
{
    [ServiceFilter(typeof(LogFilterAttribute))]
    [ApiController]
    [Route("api/books")]
    public class BooksController : ControllerBase
    {

        private readonly IServiceManager _manager;

        public BooksController(IServiceManager manager)
        {
            _manager = manager;
        }






        [HttpGet]
        public async Task<IActionResult> GetAllBooksAsync([FromQuery] BookParameters bookParameters)
        {

            var pagedResult = await _manager
                .BookService
                .GetAllBooksAsync(bookParameters, false);

            Response.Headers.Add("X-Pagination", JsonSerializer.Serialize(pagedResult.metaData));


            return Ok(pagedResult.books);


        }






        [HttpGet("{id:int}")]
        public async Task<IActionResult> GetOneBookAsync([FromRoute(Name = "id")] int id)
        {
            var book = await _manager
            .BookService
            .GetOneBookByIdAsync(id, false);
            return Ok(book);

        }





        [ServiceFilter(typeof(ValidationFilterAttribute))]
        [HttpPost]
        public async Task<IActionResult> CreateOneBookAsync([FromBody] BookDtoForInsertion bookDto)
        {
            var book = await _manager.BookService.CrateOneBookAsync(bookDto);

            return StatusCode(201, book);  //CreatedAtRoute()

        }


        [ServiceFilter(typeof(ValidationFilterAttribute))]
        [HttpPut("{id:int}")]
        public async Task<IActionResult> UpdateOneBookAsync([FromRoute(Name = "id")] int id, [FromBody] BookDtoForUpdate book)
        {
            await _manager.BookService.UpdateOneBookAsync(id, book, false);
            return NoContent(); // 204

        }



        [HttpDelete("{id:int}")]
        public async Task<IActionResult> DeleteOneBookAsync([FromRoute(Name = "id")] int id)
        {
            await _manager.BookService.DeleteOneBookAsync(id, false);
            return NoContent();

        }





        [HttpPatch("{id:int}")]
        public async Task<IActionResult> PartiallyUpdateOneBookAsync([FromRoute(Name = "id")] int id,
            [FromBody] JsonPatchDocument<BookDtoForUpdate> bookPatch)
        {

            if (bookPatch is null)
                return BadRequest(); // 400

            var result = await _manager.BookService.GetOneBookForPatchAsync(id, false);

            var bookDto = _manager
               .BookService
               .GetOneBookByIdAsync(id, true);

            bookPatch.ApplyTo(result.bookDtoForUpdate, ModelState);

            TryValidateModel(result.bookDtoForUpdate);
            if (!ModelState.IsValid)
                return UnprocessableEntity();

            await _manager.BookService.SaveChangesForPatchAsync(result.bookDtoForUpdate, result.book);


            return NoContent(); // 204

        }
    }
}

