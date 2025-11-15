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
    public class UserController : Controller
    {
        private readonly IAccountServiceForApp _serviceForApp;
        private readonly IUserService _userService;
        private readonly IMapper _mapper; 

        public UserController(IAccountServiceForApp serviceForApp, IUserService userService, IMapper mapper)
        {
            _serviceForApp = serviceForApp;
            _userService = userService;
            _mapper = mapper;
        }

        public async Task<IActionResult> Index(int pageNumber = 1, int pageSize = 20)
        {
            var ActiveUsers = await _userService.AllUserActive(1);
            var data = new HomeUser
            {
                Users = _mapper.Map<List<UserViewModel>>(ActiveUsers.Result),
                PageNumber = pageNumber,
                TotalPages = (int)Math.Ceiling((double)ActiveUsers!.Result!.Count / pageSize)
            };
            return View("Index", data);
        }

        public IActionResult Create()
        {
            return View("Create", new SaveUserViewModel
            {
                Id = "",
                FirstName = "",
                LastName = "",
                IdentificationNumber = "",
                Email = "",
                UserName = "",
                Password = "",
                ConfirmPassword = "",
                Role = "",
            });
        }


        [HttpPost]
        public async Task<IActionResult> Create(SaveUserViewModel vm) 
        {
            if (!ModelState.IsValid) 
            { 
                return View("Create", vm);
            }

            if (vm.Role == "Client" && vm.InitialAmount < 0)
            {
                ModelState.AddModelError(nameof(vm.InitialAmount), "El monto inicial no puede ser negativo.");
                return View("Create", vm);
            }

            var dto = _mapper.Map<SaveUserDto>(vm);
             await _serviceForApp.RegisterUser(dto, vm.Password);

            return RedirectToAction("Index");
        }
    }
}
