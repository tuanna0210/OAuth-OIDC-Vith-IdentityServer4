using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace MVCClient.Controllers
{
    [Authorize]
    public class HomeController : Controller
    {
        public async Task<IActionResult> Index()
        {
            var accessToken = await HttpContext.GetTokenAsync("access_token");
            var identityToken = await HttpContext.GetTokenAsync("id_token");
            ViewBag.AccessToken = accessToken;
            ViewBag.IdentityToken = identityToken;
            return View();
        }
    }
}
