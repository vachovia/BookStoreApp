using BookStoreApp.Blazor.WebAssembly.UI.Models;
using BookStoreApp.Blazor.WebAssembly.UI.Services.Base;

namespace BookStoreApp.Blazor.WebAssembly.UI.Services
{
    public interface IBookService
    {
        Task<Response<List<BookDto>>> GetBooks();
        Task<Response<BookDtoVirtualizeResponse>> GetBooks(QueryParameters queryParams);
        Task<Response<BookDetailsDto>> GetBook(int id);
        Task<Response<BookUpdateDto>> GetBookForUpdate(int id);
        Task<Response<int>> CreateBook(BookCreateDto book);
        Task<Response<int>> EditBook(int id, BookUpdateDto book);
        Task<Response<int>> DeleteBook(int id);
    }
}
