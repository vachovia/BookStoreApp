﻿using Blazored.LocalStorage;
using System.Security.Claims;
using Microsoft.AspNetCore.Components.Authorization;
using System.IdentityModel.Tokens.Jwt;
using Microsoft.IdentityModel.Tokens;

namespace BookStoreApp.Blazor.WebAssembly.UI.Providers
{
    public class ApiAuthenticationStateProvider : AuthenticationStateProvider
    {
        private readonly ILocalStorageService _localStorage;
        private readonly JwtSecurityTokenHandler _jwtSecurityTokenHandler;

        public ApiAuthenticationStateProvider(ILocalStorageService localStorage)
        {
            _localStorage = localStorage;
            _jwtSecurityTokenHandler = new JwtSecurityTokenHandler();
        }

        public override async Task<AuthenticationState> GetAuthenticationStateAsync()
        {
            var user = new ClaimsPrincipal(new ClaimsIdentity());

            var savedToken = await _localStorage.GetItemAsync<string>("accessToken");

            if (savedToken == null)
            {
                return new AuthenticationState(user);
            }

            var tokenContent = _jwtSecurityTokenHandler.ReadJwtToken(savedToken);

            if(tokenContent.ValidTo < DateTime.Now)
            {
                return new AuthenticationState(user);
            }

            //var claims = tokenContent.Claims;

            var claims = await GetClaimsAsync();

            user = new ClaimsPrincipal(new ClaimsIdentity(claims, "jwt"));

            return new AuthenticationState(user);

        }

        public async Task LoggedIn()
        {
            //var savedToken = await _localStorage.GetItemAsync<string>("accessToken");

            //var tokenContent = _jwtSecurityTokenHandler.ReadJwtToken(savedToken);

            //var claims = tokenContent.Claims.ToList();

            //claims.Add(new Claim(ClaimTypes.Name, tokenContent.Subject));

            var claims = await GetClaimsAsync();

            var user = new ClaimsPrincipal(new ClaimsIdentity(claims, "jwt"));

            var authState = Task.FromResult(new AuthenticationState(user));

            NotifyAuthenticationStateChanged(authState);
        }

        public async Task LoggedOut()
        {
            await _localStorage.RemoveItemAsync("accessToken");

            var nobody = new ClaimsPrincipal(new ClaimsIdentity());

            var authState = Task.FromResult(new AuthenticationState(nobody));

            NotifyAuthenticationStateChanged(authState);
        }

        private async Task<List<Claim>> GetClaimsAsync()
        {
            var savedToken = await _localStorage.GetItemAsync<string>("accessToken");

            var tokenContent = _jwtSecurityTokenHandler.ReadJwtToken(savedToken);

            var claims = tokenContent.Claims.ToList();

            claims.Add(new Claim(ClaimTypes.Name, tokenContent.Subject));

            return claims;
        }
    }
}
