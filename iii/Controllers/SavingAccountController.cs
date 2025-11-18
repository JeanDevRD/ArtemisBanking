using ArtemisBanking.Core.Application.Dtos.SavingsAccount;
using ArtemisBanking.Core.Application.Interfaces;
using ArtemisBanking.Core.Application.ViewModel.Loan;
using ArtemisBanking.Core.Application.ViewModels.SavingsAccount;
using AutoMapper;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace ArtemisBankingWebApp.Controllers
{
    [Authorize(Roles = "Admin")]
    public class SavingsAccountController : Controller
    {
        private readonly ISavingsAccountService _savingsService;
        private readonly IMapper _mapper;
        private readonly IAccountServiceForApp _accountServiceForApp;

        public SavingsAccountController(ISavingsAccountService savingsService, IMapper mapper, IAccountServiceForApp account)
        {
            _savingsService = savingsService;
            _mapper = mapper;
            _accountServiceForApp = account;
        }

        public async Task<IActionResult> Index(int pageNumber = 1, int pageSize = 20, string? 
            identificationNumber = null, bool? isActive = null, int? accountType = null)
        {
            var result = await _savingsService.GetSavingAccountHome(identificationNumber, pageNumber, isActive, accountType);

            if (result.IsError || result.Result == null)
            {
                TempData["Error"] = result.Message ?? "No se encontraron cuentas de ahorro";
                return View("Index", new HomeSavingsAccountViewModel
                {
                    Accounts = new List<SavingsAccountsHomeViewModel>(),
                    PageNumber = pageNumber,
                    TotalPages = 0,
                    FilterIdentificationNumber = identificationNumber,
                    IsActive = isActive,
                    AccountType = accountType
                });
            }

            var accounts = _mapper.Map<List<SavingsAccountsHomeViewModel>>(result.Result);

            var data = new HomeSavingsAccountViewModel
            {
                Accounts = accounts,
                PageNumber = pageNumber,
                TotalPages = (int)Math.Ceiling((double)result.Result.Count / pageSize),
                FilterIdentificationNumber = identificationNumber,
                IsActive = isActive,
                AccountType = accountType
            };

            return View("Index", data);
        }

        public async Task<IActionResult> Detail(string accountId)
        {
            var result = await _savingsService.GetSavingsAccountDetail(accountId);

            if (result.IsError || result.Result == null)
            {
                TempData["Error"] = result.Message ?? "Cuenta no encontrada";
                return RedirectToAction("Index");
            }

            var vm = _mapper.Map<SavingsAccountDetailViewModel>(result.Result);
            return View("Detail", vm);
        }

        public IActionResult Create(string userId)
        {
            return View("Create", new CreateSecundarySavingsAccountsViewModel
            {
                UserId = userId,
                InitialBalance = 0,
                AdminUserId = "" 
            });
        }

        [HttpPost]
        public async Task<IActionResult> Create(CreateSecundarySavingsAccountsViewModel vm)
        {
            if (!ModelState.IsValid)
                return View("Create", vm);

            var dto = _mapper.Map<CreateSecundarySavingsAccountsDto>(vm);
            var result = await _savingsService.AddSecondarySavingsAccount(dto);

            if (result.IsError)
            {
                ModelState.AddModelError(string.Empty, result.Message ?? "Error al crear la cuenta secundaria");
                return View("Create", vm);
            }

            TempData["Success"] = $"Cuenta secundaria #{result.Result.AccountNumber} creada exitosamente.";
            return RedirectToAction("Index");
        }

        public async Task<IActionResult> Cancel(int accountId)
        {
            var account = await _savingsService.GetByIdAsync(accountId);
            if (account == null)
                return RedirectToAction("Index");

            var vm = new CancelSavingsAccountViewModel
            {
                AccountId = accountId,
                AccountNumber = account.AccountNumber,
                IsSecondary = account.Type != 1, 
                CanCancel = account.Type != 1 
            };

            return View("Cancel", vm);
        }

        [HttpPost]
        public async Task<IActionResult> CancelConfirmed(string accountId)
        {
            var result = await _savingsService.CancelSecondarySavingsAccount(accountId);

            if (result.IsError)
            {
                TempData["Error"] = result.Message ?? "No se pudo cancelar la cuenta";
                return RedirectToAction("Index");
            }

            TempData["Success"] = $"Cuenta #{result.Result!.AccountNumber} cancelada exitosamente.";
            return RedirectToAction("Index");
        }

        [HttpGet]
        public async Task<IActionResult> SelectClient(string? searchCedula)
        {
            var result = await _accountServiceForApp.GetAllUser();

            var elegibleClients = result
                .Where(u => u.Role == "Client" && u.IsActive)
                .Select(u => new ElegibleUserForCreditCardViewModel
                {
                    Id = u.Id,
                    IdentificationNumber = u.IdentificationNumber,
                    FullName = $"{u.FirstName} {u.LastName}",
                    Email = u.Email,
                    MonthlyIncome = 0
                })
                .ToList();

            if (!string.IsNullOrEmpty(searchCedula))
            {
                elegibleClients = elegibleClients
                    .Where(c => c.IdentificationNumber.Contains(searchCedula))
                    .ToList();
                ViewBag.SearchCedula = searchCedula;
            }

            return View(elegibleClients);
        }

        [HttpGet]
        public IActionResult CreateSecondary(string userId)
        {
            var viewModel = new CreateSecundarySavingsAccountsViewModel
            {
                UserId = userId,
                InitialBalance = 0,
                AdminUserId = HttpContext.Session.GetString("UserId")
            };

            return View("CreateSecondary", viewModel);
        }

    }
}
