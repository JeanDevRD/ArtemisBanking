using ArtemisBanking.Core.Application.Dtos.User;
using ArtemisBanking.Core.Application.Interfaces;
using ArtemisBanking.Core.Application.ViewModels.User;
using AutoMapper;
using iTextSharp.text;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace ArtemisBankingWebApp.Controllers
{
    public class LoginController : Controller
    {
        private readonly IAccountServiceForApp _serviceForApp;
        private readonly IUserService _userService;
        private readonly IMapper _mapper; 

        public LoginController(IAccountServiceForApp serviceForApp, IUserService userService, IMapper mapper)
        {
            _serviceForApp = serviceForApp;
            _userService = userService;
            _mapper = mapper;
        }

        public async Task<IActionResult> ConfirmEmail(string userId, string token)
        {
            string response = await _serviceForApp.ConfirmAccountAsync(userId, token);
            return View("ConfirmEmail", response);
        }
    }
}
