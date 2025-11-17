using ArtemisBanking.Core.Application.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace ArtemisBankingWebApp.Controllers
{
    [Authorize(Roles = "cliente")]
    public class AccountsController : Controller
    {
        private readonly ISavingsAccountService _savingsAccountService;
        private readonly ITransactionService _transactionService;

        public AccountsController(
            ISavingsAccountService savingsAccountService,
            ITransactionService transactionService)
        {
            _savingsAccountService = savingsAccountService;
            _transactionService = transactionService;
        }

        [HttpGet]
        public async Task<IActionResult> Detail(int id)
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            var account = await _savingsAccountService.GetByIdAsync(id);

            // Verificar que la cuenta pertenece al usuario autenticado
            if (account == null || account.UserId != userId)
                return RedirectToAction("Index", "Home");

            // Obtener últimas transacciones de esta cuenta
            var transactions = await _transactionService.GetAllWithInclude();
            var accountTransactions = transactions
                .Where(t => t.SavingsAccountId == id)
                .OrderByDescending(t => t.Date)
                .Take(10)
                .ToList();

            ViewBag.Transactions = accountTransactions;

            return View(account);
        }

        [HttpGet]
        public async Task<IActionResult> Transfer(int id)
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            var account = await _savingsAccountService.GetByIdAsync(id);

            // Verificar pertenencia
            if (account == null || account.UserId != userId)
                return RedirectToAction("Index", "Home");

            // Esta acción redirige al controlador de Transferencias
            return RedirectToAction("Index", "Transfers", new { accountId = id });
        }

        [HttpGet]
        public async Task<IActionResult> Deposit(int id)
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            var account = await _savingsAccountService.GetByIdAsync(id);

            // Verificar pertenencia
            if (account == null || account.UserId != userId)
                return RedirectToAction("Index", "Home");

            ViewBag.AccountId = id;
            ViewBag.AccountNumber = account.AccountNumber;

            return View(account);
        }

        [HttpGet]
        public async Task<IActionResult> Withdraw(int id)
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            var account = await _savingsAccountService.GetByIdAsync(id);

            // Verificar pertenencia
            if (account == null || account.UserId != userId)
                return RedirectToAction("Index", "Home");

            ViewBag.AccountId = id;
            ViewBag.AccountNumber = account.AccountNumber;

            return View(account);
        }
    }
}
