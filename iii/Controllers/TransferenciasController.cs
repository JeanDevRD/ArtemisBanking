using ArtemisBanking.Core.Application.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace ArtemisBankingWebApp.Controllers
{
    [Authorize(Roles = "cliente")]
    public class TransfersController : Controller
    {
        private readonly ISavingsAccountService _savingsAccountService;
        private readonly IBeneficiaryService _beneficiaryService;
        private readonly ITransactionService _transactionService;

        public TransfersController(
            ISavingsAccountService savingsAccountService,
            IBeneficiaryService beneficiaryService,
            ITransactionService transactionService)
        {
            _savingsAccountService = savingsAccountService;
            _beneficiaryService = beneficiaryService;
            _transactionService = transactionService;
        }

        [HttpGet]
        public async Task<IActionResult> Index(int? accountId)
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);

            // Obtener cuentas del usuario
            var accounts = await _savingsAccountService.GetAllAsync();
            var myAccounts = accounts.Where(c => c.UserId == userId).ToList();

            // Si viene accountId, verificar que exista y pertenezca al usuario
            if (accountId.HasValue)
            {
                var account = myAccounts.FirstOrDefault(c => c.Id == accountId.Value);
                if (account == null)
                    return RedirectToAction("Index", "Home");

                ViewBag.SourceAccountId = accountId.Value;
                ViewBag.SourceAccountNumber = account.AccountNumber;
                ViewBag.AvailableBalance = account.Balance;
            }

            // Obtener beneficiarios del usuario
            var beneficiaries = await _beneficiaryService.GetAllAsync();
            var myBeneficiaries = beneficiaries.Where(b => b.UserId == userId).ToList();

            ViewBag.Accounts = myAccounts;
            ViewBag.Beneficiaries = myBeneficiaries;

            return View();
        }
    }
}
