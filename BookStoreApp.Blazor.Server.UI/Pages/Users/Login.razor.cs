using BookStoreApp.Blazor.Server.UI.Services.Authentication;
using BookStoreApp.Blazor.Server.UI.Services.Base;
using Microsoft.AspNetCore.Components;

namespace BookStoreApp.Blazor.Server.UI.Pages.Users
{
    public partial class Login
    {
        [Inject] IAuthenticationService _authService {  get; set; }
        [Inject] NavigationManager _navigationManager { get; set; }

        string message { get; set; } = string.Empty;
        LoginUserDto LoginModel = new LoginUserDto();

        private async Task HandleLogin()
        {
            var response = await _authService.AuthenticateAsync(LoginModel);

            if (response.Success)
            {
                NavigateToHome();
            }

            message = response.Message;
        }

        private void NavigateToHome()
        {
            _navigationManager.NavigateTo("/");
        }
    }
}
