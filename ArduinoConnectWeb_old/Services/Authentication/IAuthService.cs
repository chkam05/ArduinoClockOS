using ArduinoConnectWeb.Data.Authentication;

namespace ArduinoConnectWeb.Services.Authentication
{
    public interface IAuthService
    {

        Task<Tokens?> LoginFromApi(string? login, string? password);
        Task<bool> LoginFromWeb(string login, string password, HttpContext httpContext);

    }
}
