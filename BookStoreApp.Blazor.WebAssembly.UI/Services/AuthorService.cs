﻿using AutoMapper;
using Blazored.LocalStorage;
using BookStoreApp.Blazor.WebAssembly.UI.Models;
using BookStoreApp.Blazor.WebAssembly.UI.Services.Base;

namespace BookStoreApp.Blazor.WebAssembly.UI.Services
{
    public class AuthorService: BaseHttpService, IAuthorService
    {
        private readonly IMapper _mapper;
        private readonly IClient _client;

        public AuthorService(IClient client, ILocalStorageService localStorage, IMapper mapper) : base(client, localStorage)
        {
            _client = client;
            _mapper = mapper;
        }

        public async Task<Response<List<AuthorDto>>> GetAuthors()
        {
            Response<List<AuthorDto>> response;

            try
            {
                await GetBearerToken();
                var data = await _client.GetAllAsync();
                response = new Response<List<AuthorDto>>()
                {
                    Data = data.ToList(),
                    Success = true
                };
            }
            catch (ApiException ex)
            {
                response = ConvertApiExceptions<List<AuthorDto>>(ex);
            }

            return response;
        }

        public async Task<Response<AuthorDtoVirtualizeResponse>> GetAuthors(QueryParameters queryParams)
        {
            Response<AuthorDtoVirtualizeResponse> response;

            try
            {
                await GetBearerToken();
                var data = await _client.AuthorsGETAsync(queryParams.StartIndex, queryParams.PageSize);
                response = new Response<AuthorDtoVirtualizeResponse>()
                {
                    Data = data                    ,
                    Success = true
                };
            }
            catch(ApiException ex)
            {
                response = ConvertApiExceptions<AuthorDtoVirtualizeResponse>(ex);
            }

            return response;
        }

        public async Task<Response<int>> CreateAuthor(AuthorCreateDto author)
        {
            Response<int> response= new();

            try
            {
                await GetBearerToken();
                await _client.AuthorsPOSTAsync(author);
            }
            catch (ApiException ex)
            {
                response = ConvertApiExceptions<int>(ex);
            }

            return response;
        }

        public async Task<Response<int>> EditAuthor(int id, AuthorUpdateDto author)
        {
            Response<int> response = new();

            try
            {
                await GetBearerToken();
                await _client.AuthorsPUTAsync(id, author);
            }
            catch (ApiException ex)
            {
                response = ConvertApiExceptions<int>(ex);
            }

            return response;
        }

        public async Task<Response<AuthorDetailsDto>> GetAuthor(int id)
        {
            Response<AuthorDetailsDto> response;

            try
            {
                await GetBearerToken();
                var data = await _client.AuthorsGET2Async(id);
                response = new Response<AuthorDetailsDto>()
                {
                    Data = data,
                    Success = true
                };
            }
            catch (ApiException ex)
            {
                response = ConvertApiExceptions<AuthorDetailsDto>(ex);
            }

            return response;
        }

        public async Task<Response<AuthorUpdateDto>> GetAuthorForUpdate(int id)
        {
            Response<AuthorUpdateDto> response;

            try
            {
                await GetBearerToken();
                var data = await _client.AuthorsGET2Async(id);
                response = new Response<AuthorUpdateDto>()
                {
                    Data = _mapper.Map<AuthorUpdateDto>(data),
                    Success = true
                };
            }
            catch (ApiException ex)
            {
                response = ConvertApiExceptions<AuthorUpdateDto>(ex);
            }

            return response;
        }

        public async Task<Response<int>> DeleteAuthor(int authorId)
        {
            Response<int> response = new();

            try
            {
                await GetBearerToken();
                await _client.AuthorsDELETEAsync(authorId);
            }
            catch (ApiException ex)
            {
                response = ConvertApiExceptions<int>(ex);
            }

            return response;
        }
    }
}
