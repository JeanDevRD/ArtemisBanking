using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using ArtemisBanking.Infraestructure.Identity.Entities;

namespace ArtemisBankingWebApp.Controllers
{
    public class AuthController : Controller
    {
        private readonly SignInManager<User> _signInManager;
        private readonly UserManager<User> _userManager;

        public AuthController(SignInManager<User> signInManager, UserManager<User> userManager)
        {
            _signInManager = signInManager;
            _userManager = userManager;
        }

        [HttpGet]
        [AllowAnonymous]
        public IActionResult Login()
        {
            if (_signInManager.IsSignedIn(User))
            {
                if (User.IsInRole("Admin"))
                    return RedirectToAction("Index", "HomeAdmin");
                if (User.IsInRole("Cliente"))
                    return RedirectToAction("Index", "Home");
                if (User.IsInRole("Teller"))
                    return RedirectToAction("Index", "Cajero");
                if (User.IsInRole("Merchant"))
                    return RedirectToAction("Index", "Merchant");
            }
            return View();
        }

        [HttpPost]
        [AllowAnonymous]
        public async Task<IActionResult> Login(string username, string password)
        {
            var user = await _userManager.FindByNameAsync(username);
            if (user == null)
            {
                ViewBag.Error = "Usuario no encontrado.";
                return View();
            }

            var result = await _signInManager.PasswordSignInAsync(user, password, true, lockoutOnFailure: false);

            if (!result.Succeeded)
            {
                ViewBag.Error = "Credenciales incorrectas.";
                return View();
            }

            var roles = await _userManager.GetRolesAsync(user);

            if (roles.Contains("Admin"))
                return RedirectToAction("Index", "HomeAdmin");

            if (roles.Contains("Cliente"))
                return RedirectToAction("Index", "Home");

            if (roles.Contains("Teller"))
                return RedirectToAction("Index", "Teller");

            if (roles.Contains("Merchant"))
                return RedirectToAction("Index", "Merchant");

            await _signInManager.SignOutAsync();
            return RedirectToAction("AccesoDenegado");
        }

        [Authorize]
        public async Task<IActionResult> Logout()
        {
            await _signInManager.SignOutAsync();
            return RedirectToAction("Login");
        }

        [AllowAnonymous]
        public IActionResult AccesoDenegado() => View();

        [AllowAnonymous]
        public IActionResult SessionExpirada()
        {
            TempData["Error"] = "Tu sesión ha expirado. Inicia sesión nuevamente.";
            return RedirectToAction("Login");
        }

    }
}
