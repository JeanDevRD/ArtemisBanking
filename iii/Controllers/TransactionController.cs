using ArtemisBanking.Core.Application.Dtos.Transaction;
using ArtemisBanking.Core.Application.Interfaces;
using ArtemisBanking.Core.Application.ViewModels.Transaction;
using AutoMapper;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace ArtemisBankingWebApp.Controllers
{
    public class TransactionController : Controller
    {
        private readonly ITransactionService _transactionService;
        private readonly IMapper _mapper;

        public TransactionController(ITransactionService transactionService, IMapper mapper)
        {
            _transactionService = transactionService;
            _mapper = mapper;
        }

        public IActionResult Index()
        {
            return RedirectToAction("Index", "CasherDashboard");
        }

        public IActionResult Deposit()
        {
            return View("Deposit", new DepositTransactionViewModel());
        }

        [HttpPost]
        public async Task<IActionResult> Deposit(DepositTransactionViewModel vm)
        {
            if (!ModelState.IsValid)
                return View("Deposit", vm);

            var dto = _mapper.Map<DepositTransactionDto>(vm);
            var validation = await _transactionService.ValidateTransactionAsync(dto, IsWithdrawal: false);

            if (validation.IsError || validation.Result == null)
            {
                ModelState.AddModelError(string.Empty, validation.Message ?? "Número de cuenta no válido o cuenta inactiva.");
                return View("Deposit", vm);
            }

            var confirmVm = _mapper.Map<TransactionConfirmViewModel>(validation.Result);
            return View("ConfirmDeposit", confirmVm);
        }

        [HttpPost]
        public async Task<IActionResult> ConfirmDeposit(TransactionConfirmViewModel confirmVm)
        {
            if (!ModelState.IsValid)
                return View("ConfirmDeposit", confirmVm);

            var cashierId = User.FindFirstValue(ClaimTypes.NameIdentifier) ?? string.Empty;

            var dto = new DepositTransactionDto
            {
                AccountNumber = confirmVm.AccountNumber,
                Amount = confirmVm.Amount
            };

            var ok = await _transactionService.ConfirmDepositAsync(dto, cashierId);
            if (!ok)
            {
                TempData["Error"] = "No se pudo procesar el depósito.";
                return RedirectToAction("Deposit");
            }

            TempData["Success"] = $"Depósito realizado a la cuenta {MaskAccount(confirmVm.AccountNumber)} por {confirmVm.Amount:C}.";
            return RedirectToAction("Index", "CasherDashboard");
        }

        public IActionResult Withdrawal()
        {
            return View("Withdrawal", new WithdrawalTransactionViewModel());
        }

        [HttpPost]
        public async Task<IActionResult> Withdrawal(WithdrawalTransactionViewModel vm)
        {
            if (!ModelState.IsValid)
                return View("Withdrawal", vm);

            var depositDto = new DepositTransactionDto
            {
                AccountNumber = vm.AccountNumber,
                Amount = vm.Amount
            };

            var validation = await _transactionService.ValidateTransactionAsync(depositDto, IsWithdrawal: true);

            if (validation.IsError || validation.Result == null)
            {
                ModelState.AddModelError(string.Empty, validation.Message ?? "Cuenta inválida, inactiva o fondos insuficientes.");
                return View("Withdrawal", vm);
            }

            var confirmVm = _mapper.Map<TransactionConfirmViewModel>(validation.Result);
            return View("ConfirmWithdrawal", confirmVm);
        }

        [HttpPost]
        public async Task<IActionResult> ConfirmWithdrawal(TransactionConfirmViewModel confirmVm)
        {
            if (!ModelState.IsValid)
                return View("ConfirmWithdrawal", confirmVm);

            var cashierId = User.FindFirstValue(ClaimTypes.NameIdentifier) ?? string.Empty;

            var dto = new WithdrawalTransactionDto
            {
                AccountNumber = confirmVm.AccountNumber,
                Amount = confirmVm.Amount
            };

            var ok = await _transactionService.ConfirmWithdrawalAsync(dto, cashierId);
            if (!ok)
            {
                TempData["Error"] = "No se pudo procesar el retiro.";
                return RedirectToAction("Withdrawal");
            }

            TempData["Success"] = $"Retiro realizado desde la cuenta {MaskAccount(confirmVm.AccountNumber)} por {confirmVm.Amount:C}.";
            return RedirectToAction("Index", "CasherDashboard");
        }

      
        [HttpPost]
        public IActionResult CancelOperation(string returnAction = "Index", string returnController = "CasherDashboard")
        {
            TempData["Info"] = "Operación cancelada.";
            return RedirectToAction(returnAction, returnController);
        }


        private static string MaskAccount(string accountNumber)
        {
            if (string.IsNullOrWhiteSpace(accountNumber) || accountNumber.Length < 4)
                return "xxxx-xxx-xxxx";
            var last4 = accountNumber[^4..];
            return $"xxx-xxx-{last4}";
        }
    }
}
