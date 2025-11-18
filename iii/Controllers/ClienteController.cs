using ArtemisBanking.Core.Application.Interfaces;
using ArtemisBanking.Core.Application.Dtos.CreditCard;
using ArtemisBanking.Core.Application.Dtos.Loan;
using ArtemisBanking.Core.Application.Dtos.SavingsAccount;
using ArtemisBanking.Core.Application.Dtos.Transaction;
using ArtemisBanking.Core.Application.ViewModels.Cliente;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace ArtemisBankingWebApp.Controllers
{
    [Authorize(Roles = "cliente")]
    public class ClientController : Controller
    {
        private readonly ISavingsAccountService _savingsService;
        private readonly ILoanService _loanService;
        private readonly ICreditCardService _cardService;
        private readonly ITransactionService _transactionService;

        public ClientController(
            ISavingsAccountService savingsService,
            ILoanService loanService,
            ICreditCardService cardService,
            ITransactionService transactionService)
        {
            _savingsService = savingsService;
            _loanService = loanService;
            _cardService = cardService;
            _transactionService = transactionService;
        }

        public async Task<IActionResult> Home()
        {
            var userId = User.FindFirstValue(System.Security.Claims.ClaimTypes.NameIdentifier);
            if (string.IsNullOrEmpty(userId))
                return RedirectToAction("Login", "Auth");

            var accounts = await _savingsService.GetAllAsync();
            var loans = await _loanService.GetAllAsync();
            var cards = await _cardService.GetAllAsync();
            var transactions = await _transactionService.GetAllAsync();

            // Filtrar por usuario
            var myAccounts = accounts.Where(c => c.UserId == userId).ToList();
            var myLoans = loans.Where(p => p.UserId == userId).ToList();
            var myCards = cards.Where(t => t.UserId == userId).ToList();

            var last5 = transactions
                .Where(t => t.SavingsAccount != null && t.SavingsAccount.UserId == userId)
                .OrderByDescending(t => t.Date)
                .Take(5)
                .ToList();

            decimal totalBalance = myAccounts.Sum(c => c.Balance);
            decimal cardDebt = myCards.Sum(t => t.CurrentDebt);
            decimal loanDebt = myLoans.Sum(p => p.Amount);
            decimal totalDebt = cardDebt + loanDebt;

            decimal minimumPayment = myCards
                .Where(t => t.CurrentDebt > 0)
                .Sum(t => t.CurrentDebt * 0.05m);

            decimal nextLoanPayment = myLoans
                .Where(p => p.Installments != null && p.Installments.Any(i => !i.IsPaid))
                .SelectMany(p => p.Installments!)
                .Where(i => !i.IsPaid)
                .OrderBy(i => i.DueDate)
                .Select(i => i.PaymentAmount)
                .FirstOrDefault();

            var vm = new ClientHomeViewModel
            {
                Accounts = myAccounts,
                Loans = myLoans,
                Cards = myCards,
                LastTransactions = last5,
                TotalBalance = totalBalance,
                TotalDebt = totalDebt,
                MinimumPayment = minimumPayment,
                NextInstallments = nextLoanPayment
            };

            return View(vm);
        }
    }
}

