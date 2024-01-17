using AutoMapper;
using Blazored.LocalStorage;
using BookStoreApp.Blazor.WebAssembly.UI.Models;
using BookStoreApp.Blazor.WebAssembly.UI.Services.Base;

namespace BookStoreApp.Blazor.WebAssembly.UI.Services
{
    public class BookService: BaseHttpService, IBookService
    {
        private readonly IMapper _mapper;
        private readonly IClient _client;

        public BookService(IClient client, ILocalStorageService localStorage, IMapper mapper) : base(client, localStorage)
        {
            _client = client;
            _mapper = mapper;
        }

        public async Task<Response<List<BookDto>>> GetBooks()
        {
            Response<List<BookDto>> response;

            try
            {
                await GetBearerToken();
                var data = await _client.GetAll2Async();
                response = new Response<List<BookDto>>()
                {
                    Data = data.ToList(),
                    Success = true
                };
            }
            catch (ApiException ex)
            {
                response = ConvertApiExceptions<List<BookDto>>(ex);
            }

            return response;
        }

        public async Task<Response<BookDtoVirtualizeResponse>> GetBooks(QueryParameters queryParams)
        {
            Response<BookDtoVirtualizeResponse> response;

            try
            {
                await GetBearerToken();
                var data = await _client.BooksGETAsync(queryParams.StartIndex, queryParams.PageSize);
                response = new Response<BookDtoVirtualizeResponse>()
                {
                    Data = data,
                    Success = true
                };
            }
            catch (ApiException ex)
            {
                response = ConvertApiExceptions<BookDtoVirtualizeResponse>(ex);
            }

            return response;
        }

        public async Task<Response<int>> CreateBook(BookCreateDto book)
        {
            Response<int> response = new();

            try
            {
                await GetBearerToken();
                await _client.BooksPOSTAsync(book);
            }
            catch (ApiException ex)
            {
                response = ConvertApiExceptions<int>(ex);
            }

            return response;
        }

        public async Task<Response<int>> EditBook(int id, BookUpdateDto book)
        {
            Response<int> response = new();

            try
            {
                await GetBearerToken();
                await _client.BooksPUTAsync(id, book);
            }
            catch (ApiException ex)
            {
                response = ConvertApiExceptions<int>(ex);
            }

            return response;
        }

        public async Task<Response<BookDetailsDto>> GetBook(int id)
        {
            Response<BookDetailsDto> response;

            try
            {
                await GetBearerToken();
                var data = await _client.BooksGET2Async(id);
                response = new Response<BookDetailsDto>()
                {
                    Data = data,
                    Success = true
                };
            }
            catch (ApiException ex)
            {
                response = ConvertApiExceptions<BookDetailsDto>(ex);
            }

            return response;
        }

        public async Task<Response<BookUpdateDto>> GetBookForUpdate(int id)
        {
            Response<BookUpdateDto> response;

            try
            {
                await GetBearerToken();
                var data = await _client.BooksGET2Async(id);
                response = new Response<BookUpdateDto>()
                {
                    Data = _mapper.Map<BookUpdateDto>(data),
                    Success = true
                };
            }
            catch (ApiException ex)
            {
                response = ConvertApiExceptions<BookUpdateDto>(ex);
            }

            return response;
        }

        public async Task<Response<int>> DeleteBook(int bookId)
        {
            Response<int> response = new();

            try
            {
                await GetBearerToken();
                await _client.BooksDELETEAsync(bookId);
            }
            catch (ApiException ex)
            {
                response = ConvertApiExceptions<int>(ex);
            }

            return response;
        }
    }
}
