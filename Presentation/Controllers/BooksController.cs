using Entities.DataTransferObject;
using Entities.Models;
using Microsoft.AspNetCore.JsonPatch;
using Microsoft.AspNetCore.Mvc;
using Services.Contracts;
using static Entities.Exceptions.NotFound;

namespace Presentation.Controllers
{
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
        public IActionResult GetAllBooks()
        {

            var books = _manager.BookService.GetAllBooks(false);
            return Ok(books);


        }
        [HttpGet("{id:int}")]
        public IActionResult GetOneBook([FromRoute(Name = "id")] int id)
        {
            var book = _manager
            .BookService
            .GetOneBookById(id, false);
            return Ok(book);

        }

        [HttpPost]
        public IActionResult CreateOneBook([FromBody] Book book)
        {
            if (book is null)
                return BadRequest(); //400
            _manager.BookService.CrateOneBook(book);


            return StatusCode(201, book);

        }
        [HttpPut("{id:int}")]
        public IActionResult UpdateOneBook([FromRoute(Name = "id")] int id, [FromBody] BookDtoForUpdate book)
        {
            if (book is null)
                return BadRequest(); //400
            _manager.BookService.UpdateOneBook(id, book, true);
            return NoContent(); // 204


        }

        [HttpDelete("{id:int}")]
        public IActionResult DeleteOneBook([FromRoute(Name = "id")] int id)
        {
            _manager.BookService.DeleteOneBook(id, false);
            return NoContent();

        }

        [HttpPatch("{id:int}")]
        public IActionResult PartiallyUpdateOneBook([FromRoute(Name = "id")] int id, [FromBody] JsonPatchDocument<Book> bookPatch)
        {
            var entity = _manager
               .BookService
               .GetOneBookById(id, true);

            bookPatch.ApplyTo(entity);
            _manager.BookService.UpdateOneBook(id,
                new BookDtoForUpdate(entity.Id, entity.Title, entity.Price),
                true);

            return NoContent();

        }
    }
}

