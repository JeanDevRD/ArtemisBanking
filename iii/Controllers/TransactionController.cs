using ArtemisBanking.Core.Application.Dtos.Transaction;
using ArtemisBanking.Core.Application.Interfaces;
using ArtemisBanking.Core.Application.ViewModels.Transaction;
using AutoMapper;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace ArtemisBankingWebApp.Controllers
{
    [Authorize(Roles = "Teller")]
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
            return View();
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

        public IActionResult PayCreditCard()
        {
            return View("PayCreditCard", new PayCreditCardViewModel());
        }

        [HttpPost]
        public async Task<IActionResult> PayCreditCard(PayCreditCardViewModel vm)
        {
            if (!ModelState.IsValid)
                return View("PayCreditCard", vm);

            var validation = await _transactionService.ValidateCreditCardPaymentAsync(
                vm.AccountNumber, vm.CardNumber, vm.Amount);

            if (validation.IsError)
            {
                ModelState.AddModelError(string.Empty, validation.Message ?? "Error en validación");
                return View("PayCreditCard", vm);
            }


            var confirmVm = new ConfirmPayCreditCardViewModel
            {
                AccountNumber = vm.AccountNumber,
                Amount = vm.Amount,
                CardNumber = vm.CardNumber,
                CardHolderName = validation.Result!.HolderName,
                LastFourDigits = vm.CardNumber.Substring(12)
            };

            return View("ConfirmPayCreditCard", confirmVm);
        }

        [HttpPost]
        public async Task<IActionResult> ConfirmPayCreditCard(ConfirmPayCreditCardViewModel confirmVm)
        {
            var cashierId = User.FindFirstValue(ClaimTypes.NameIdentifier) ?? string.Empty;
            var ok = await _transactionService.ConfirmCreditCardPaymentAsync(
                confirmVm.AccountNumber, confirmVm.CardNumber, confirmVm.Amount, cashierId);

            if (!ok)
            {
                TempData["Error"] = "Error al procesar el pago.";
                return RedirectToAction("PayCreditCard");
            }

            TempData["Success"] = $"Pago de {confirmVm.Amount:C} realizado a ****{confirmVm.LastFourDigits}.";
            return RedirectToAction("Index", "CasherDashboard");
        }

        private static string MaskAccount(string accountNumber)
        {
            if (string.IsNullOrWhiteSpace(accountNumber) || accountNumber.Length < 4)
                return "xxxx-xxx-xxxx";
            var last4 = accountNumber[^4..];
            return $"xxx-xxx-{last4}";
        }

        public async Task<IActionResult> PayLoan()
        {
            return View("PayLoan", new PayLoanViewModel());
        }

        [HttpPost]
        public async Task<IActionResult> PayLoan(PayLoanViewModel vm)
        {
            if (!ModelState.IsValid)
            {
                return View("PayLoan", vm);
            }

            var validation = await _transactionService.ValidateLoanPaymentAsync(
                vm.AccountNumber, vm.LoanNumber, vm.Amount);

            if (validation.IsError || validation.Result == null)
            {
                ModelState.AddModelError(string.Empty,
                    validation.Message ?? "Error en la validación del pago");
                return View("PayLoan", vm);
            }

            var confirmVm = new ConfirmPayLoanViewModel
            {
                AccountNumber = validation.Result.AccountNumber,
                Amount = validation.Result.Amount,
                LoanNumber = vm.LoanNumber,
                LoanHolderName = validation.Result.HolderName,
                OutstandingAmount = validation.Result.OutstandingAmount
            };

            return View("ConfirmPayLoan", confirmVm);
        }

        [HttpPost]
        public async Task<IActionResult> ConfirmPayLoan(ConfirmPayLoanViewModel confirmVm)
        {
            if (!ModelState.IsValid)
            {
                return View("ConfirmPayLoan", confirmVm);
            }

            var cashierId = User.FindFirstValue(ClaimTypes.NameIdentifier) ?? string.Empty;

            var ok = await _transactionService.ConfirmLoanPaymentAsync(
                confirmVm.AccountNumber, confirmVm.LoanNumber, confirmVm.Amount, cashierId);

            if (!ok)
            {
                TempData["Error"] = "No se pudo procesar el pago al préstamo.";
                return RedirectToAction("PayLoan");
            }

            TempData["Success"] = $"Pago realizado al préstamo {confirmVm.LoanNumber} por {confirmVm.Amount:C}.";
            return RedirectToAction("Index", "CasherDashboard");
        }

        public IActionResult ThirdPartyTransaction()
        {
            return View("ThirdPartyTransaction", new ThirdPartyTransactionViewModel());
        }

        [HttpPost]
        public async Task<IActionResult> ThirdPartyTransaction(ThirdPartyTransactionViewModel vm)
        {
            if (!ModelState.IsValid)
                return View("ThirdPartyTransaction", vm);

            var validation = await _transactionService.ValidateThirdPartyTransactionAsync(
                vm.AccountOrigin, vm.AccountDestination, vm.Amount);

            if (validation.IsError || validation.Result == null)
            {
                ModelState.AddModelError(string.Empty,
                    validation.Message ?? "Error en la validación de la transacción");
                return View("ThirdPartyTransaction", vm);
            }

            var confirmVm = new ConfirmThirdPartyTransactionViewModel
            {
                AccountOrigin = vm.AccountOrigin,
                AccountDestination = vm.AccountDestination,
                Amount = vm.Amount,
                DestinationHolderName = validation.Result.HolderName
            };

            return View("ConfirmThirdPartyTransaction", confirmVm);
        }

        [HttpPost]
        public async Task<IActionResult> ConfirmThirdPartyTransaction(ConfirmThirdPartyTransactionViewModel confirmVm)
        {
            if (!ModelState.IsValid)
                return View("ConfirmThirdPartyTransaction", confirmVm);

            var cashierId = User.FindFirstValue(ClaimTypes.NameIdentifier) ?? string.Empty;

            var ok = await _transactionService.ConfirmThirdPartyTransactionAsync(
                confirmVm.AccountOrigin, confirmVm.AccountDestination, confirmVm.Amount, cashierId);

            if (!ok)
            {
                TempData["Error"] = "No se pudo procesar la transacción.";
                return RedirectToAction("ThirdPartyTransaction");
            }

            var lastFourDest = confirmVm.AccountDestination[^4..];
            TempData["Success"] = $"Transacción realizada a la cuenta ****{lastFourDest} por {confirmVm.Amount:C}.";
            return RedirectToAction("Index", "CasherDashboard");
        }

    }
}
