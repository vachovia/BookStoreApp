using BookStoreApp.Blazor.Server.UI.Models;
using BookStoreApp.Blazor.Server.UI.Services.Base;

namespace BookStoreApp.Blazor.Server.UI.Services
{
    public interface IAuthorService
    {
        Task<Response<List<AuthorDto>>> GetAuthors();
        Task<Response<AuthorDtoVirtualizeResponse>> GetAuthors(QueryParameters queryParams);
        Task<Response<AuthorDetailsDto>> GetAuthor(int id);
        Task<Response<AuthorUpdateDto>> GetAuthorForUpdate(int id);
        Task<Response<int>> CreateAuthor(AuthorCreateDto author);
        Task<Response<int>> EditAuthor(int id, AuthorUpdateDto author);
        Task<Response<int>> DeleteAuthor(int id);
    }
}