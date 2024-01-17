using BookStoreApp.API.Data;
using BookStoreApp.API.Models.Book;
using Microsoft.AspNetCore.Mvc;

namespace BookStoreApp.API.Repositories
{
    public interface IBookRepository : IGenericRepository<Book>
    {
        Task<List<BookDto>> GetBookWithAuthorsAsync();
        Task<BookDetailsDto> GetBookAsync(int id);
    }
}
