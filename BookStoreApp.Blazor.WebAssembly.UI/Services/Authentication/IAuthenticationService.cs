﻿using BookStoreApp.Blazor.WebAssembly.UI.Services.Base;

namespace BookStoreApp.Blazor.WebAssembly.UI.Services.Authentication
{
    public interface IAuthenticationService
    {
        Task<Response<AuthResponse>> AuthenticateAsync(LoginUserDto loginModel);
        Task Logout();
        Task RegisterAsync(UserDto registrationModel);
    }
}
