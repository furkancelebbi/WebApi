﻿using AutoMapper;
using Entities.DataTransferObject;
using Entities.Exceptions;
using Entities.Models;
using Entities.RequestFeatures;
using NLog;
using Repositories.Contracts;
using Services.Contracts;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static Entities.Exceptions.NotFound;

namespace Services
{
    public class BookManager : IBookService
    {
        private readonly IRepositoryManager _manager;
        private readonly ILoggerService _logger;
        private readonly IMapper _mapper;

        public BookManager(IRepositoryManager manager, ILoggerService logger, IMapper mapper)
        {
            _manager = manager;
            _logger = logger;
            _mapper = mapper;
        }


        public async Task<BookDto> CrateOneBookAsync(BookDtoForInsertion bookDto)
        {
            var entity = _mapper.Map<Book>(bookDto);
            _manager.Book.CreateOneBook(entity);
            await _manager.SaveAsync();
            return _mapper.Map<BookDto>(entity);
        }



        public async Task<(IEnumerable<BookDto> books, MetaData metaData)>
            GetAllBooksAsync(BookParameters bookParameters,
            bool trackChanges)
        {
            if (!bookParameters.ValidPriceRange)
                throw new PriceOutofRangeBadRequestException();

            var booksWithMetaData = await _manager
                .Book
                .GetAllBooksAsync(bookParameters, trackChanges);
            var booksDto = _mapper.Map<IEnumerable<BookDto>>(booksWithMetaData);

            return (booksDto, booksWithMetaData.MetaData);
        }




        public async Task<BookDto> GetOneBookByIdAsync(int id, bool trackChanges)
        {
            var book = await GetOneBooByIdAndCheckExits(id, trackChanges);
            return _mapper.Map<BookDto>(book);
        }




        public async Task<(BookDtoForUpdate bookDtoForUpdate, Book book)>
            GetOneBookForPatchAsync(int id, bool trackChanges)
        {
            var book = await GetOneBooByIdAndCheckExits(id, trackChanges);

            var bookDtoForUpdate = _mapper.Map<BookDtoForUpdate>(book);

            return (bookDtoForUpdate, book);
        }




        public async Task DeleteOneBookAsync(int id, bool trackChanges)
        {

            var entity = await GetOneBooByIdAndCheckExits(id, trackChanges);
            _manager.Book.DeleteOneBook(entity);
            await _manager.SaveAsync();


        }



        public async Task SaveChangesForPatchAsync(BookDtoForUpdate bookDtoForUpdate, Book book)
        {

            _mapper.Map(bookDtoForUpdate, book);
            await _manager.SaveAsync();

        }


        public async Task UpdateOneBookAsync(int id,
           BookDtoForUpdate bookDto,
           bool trackChanges)
        {
            //check entity
            var entity = await GetOneBooByIdAndCheckExits(id, trackChanges);

            entity = _mapper.Map<Book>(bookDto);

            _manager.Book.Update(entity);
            await _manager.SaveAsync();

        }


        private async Task<Book> GetOneBooByIdAndCheckExits(int id, bool trackChanges)
        {
            var entity = await _manager.Book.GetOneBooksByIdAsync(id, trackChanges);

            if (entity is null)
                throw new BookNotFoundException(id);

            return entity;
        }

    }
}




