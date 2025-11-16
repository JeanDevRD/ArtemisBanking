using ArtemisBanking.Core.Application.Dtos.User;
using ArtemisBanking.Core.Application.Interfaces;
using ArtemisBanking.Core.Application.ViewModels.User;
using ArtemisBanking.Infraestructure.Identity.Entities;
using AutoMapper;
using iTextSharp.text;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace ArtemisBankingWebApp.Controllers
{
    public class LoginController : Controller
    {
        private readonly IAccountServiceForApp _serviceForApp;
        private readonly IMapper _mapper;
        private readonly UserManager<User> _userManager;
        private readonly SignInManager<User> _signInManager;

        public LoginController(IAccountServiceForApp serviceForApp, IUserService userService, UserManager<User> userManager, IMapper mapper, SignInManager<User> signInManager)
        {
            _serviceForApp = serviceForApp;
            _mapper = mapper;
            _userManager = userManager;
            _signInManager = signInManager;
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
            return RedirectToAction("AccessDenied");
        }

        [Authorize]
        public async Task<IActionResult> Logout()
        {
            await _signInManager.SignOutAsync();
            return RedirectToAction("Login");
        }
        public async Task<IActionResult> AccessDenied() 
        {
            User? userSession = await _userManager.GetUserAsync(User);

            if (userSession != null)
            {
                var user = await _serviceForApp.GetUserByUserName(userSession.UserName!);
                return View();
            }

            return RedirectToRoute(new { controller = "Login", action = "Index" });
        }

        public IActionResult SessionExpired()
        {
            TempData["Error"] = "Tu sesión ha expirado. Inicia sesión nuevamente.";
            return RedirectToAction("Login");
        }

        public async Task<IActionResult> ConfirmEmail(string userId, string token)
        {
            string response = await _serviceForApp.ConfirmAccountAsync(userId, token);
            return View("ConfirmEmail", response);
        }

        [HttpPost]
        public async Task<IActionResult> ResetPassword(ResetPasswordRequestViewModel vm)
        {
            if (!ModelState.IsValid)
            {
                return View(vm);
            }

            ResetPasswordRequestDto dto = _mapper.Map<ResetPasswordRequestDto>(vm);

            UserResponseDto? returnUser = await _serviceForApp.ResetPasswordAsync(dto);

            if (returnUser.HasError)
            {
                ViewBag.HasError = true;
                ViewBag.Errors = returnUser.Errors;
                return View(vm);
            }

            return RedirectToRoute(new { controller = "Login", action = "Index" });
        }




    }
}
