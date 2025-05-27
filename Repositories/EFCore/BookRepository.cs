using Entities.Models;
using Entities.RequestFeatures;
using Microsoft.EntityFrameworkCore;
using Repositories.Contracts;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WebApi.Repositories;

namespace Repositories.EFCore
{
    public class _bookRepository : RepositoryBase<Book>, IBookRepository
    {
        public _bookRepository(RepositoryContext context) : base(context)
        {

        }

        public void CreateOneBook(Book book) => Create(book);


        public void DeleteOneBook(Book book) => Delete(book);


        public async Task<PagedList<Book>> GetAllBooksAsync(BookParameters bookParameters,
            bool trackChanges)
        {
            var books = await FindByCondition(b =>
            (b.Price >= bookParameters.MinPrice) &&
            (b.Price <= bookParameters.MaxPrice), trackChanges)
            .OrderBy(b => b.Id)
            .ToListAsync();

            return PagedList<Book>
                .ToPagedList(books,
                bookParameters.PageNumber,
                bookParameters.PageSize);
        }


        public async Task<Book> GetOneBooksByIdAsync(int id, bool trackChanges) =>
          await FindByCondition(b => b.Id.Equals(id), trackChanges)
            .SingleOrDefaultAsync();



        public void UpdateOneBook(Book book) => Update(book);

    }
}
