using ArduinoConnectWeb.Services.Authentication;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace ArduinoConnectWeb.Controllers
{
    public class IndexController : Controller
    {

        //  VARIABLES

        private readonly IAuthService _authService;
        private readonly IConfiguration _configuration;


        //  METHODS

        #region CLASS METHODS

        //  --------------------------------------------------------------------------------
        public IndexController(IAuthService authService, IConfiguration configuration)
        {
            _authService = authService;
            _configuration = configuration;
        }

        #endregion CLASS METHODS

        #region PAGE METHODS

        //  --------------------------------------------------------------------------------
        public async Task<IActionResult> HomePage()
        {
            ViewBag.Login = HttpContext.User.Identity?.Name;
            return View();
        }

        //  --------------------------------------------------------------------------------
        public async Task<IActionResult> LoginPage()
        {
            return View();
        }

        //  --------------------------------------------------------------------------------
        public async Task<IActionResult> DashboardPage()
        {
            ViewBag.Login = HttpContext.User.Identity?.Name;
            return View();
        }

        #endregion PAGE METHODS

        #region REQUEST METHODS

        //  --------------------------------------------------------------------------------
        [HttpPost]
        public async Task<IActionResult> LoginPage(string login, string password)
        {
            var isSuccess = await _authService.LoginFromWeb(login, password, HttpContext);
            return isSuccess ? RedirectToAction("DashboardPage") : View();
        }

        //  --------------------------------------------------------------------------------
        public async Task<IActionResult> Logout()
        {
            await HttpContext.SignOutAsync(CookieAuthenticationDefaults.AuthenticationScheme);
            return RedirectToAction("HomePage");
        }

        #endregion REQUEST METHODS

    }
}
