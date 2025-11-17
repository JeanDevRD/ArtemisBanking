using ArtemisBanking.Core.Application.Interfaces;
using ArtemisBankingWebApp.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;
using X.PagedList;
using X.PagedList.Mvc.Core;

namespace ArtemisBankingWebApp.Controllers
{
    [Authorize(Roles = "cliente")]
    public class HomeController : Controller
    {
        private readonly ISavingsAccountService _savingsAccountService;
        private readonly ILoanService _loanService;
        private readonly ICreditCardService _creditCardService;
        private readonly ITransactionService _transactionService;

        public HomeController(
            ISavingsAccountService savingsAccountService,
            ILoanService loanService,
            ICreditCardService creditCardService,
            ITransactionService transactionService)
        {
            _savingsAccountService = savingsAccountService;
            _loanService = loanService;
            _creditCardService = creditCardService;
            _transactionService = transactionService;
        }

        public async Task<IActionResult> Index(int? page)
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            if (string.IsNullOrEmpty(userId))
                return RedirectToAction("Login", "Auth");

            var pageNumber = page ?? 1;
            var pageSize = 20;

            var accounts = await _savingsAccountService.GetAllAsync();
            var loans = await _loanService.GetAllAsync();
            var cards = await _creditCardService.GetAllAsync();

            var myAccounts = accounts.Where(c => c.UserId == userId).ToList();
            var myLoans = loans.Where(p => p.UserId == userId).ToList();
            var myCards = cards.Where(t => t.UserId == userId).ToList();

            // ORDEN: más reciente → antiguo
            var products = myAccounts.Select(c => new ProductViewModel
            {
                Type = "Savings Account",
                Number = c.AccountNumber,
                Amount = c.Balance,
                Status = c.IsActive ? "Active" : "Inactive",
                Date = c.CreatedAt,
                Id = c.Id,
                TypeId = 1
            })
            .Concat(myLoans.Select(p => new ProductViewModel
            {
                Type = "Loan",
                Number = p.LoanNumber,
                Amount = p.Amount,
                Status = p.IsActive ? "Active" : "Closed",
                Date = p.ApprovedAt,
                Id = p.Id,
                TypeId = 2
            }))
            .Concat(myCards.Select(t => new ProductViewModel
            {
                Type = "Credit Card",
                Number = t.CardNumber,
                Amount = t.CurrentDebt,
                Status = t.IsActive ? "Active" : "Inactive",
                Date = t.CreateAt,
                Id = t.Id,
                TypeId = 3
            }))
            .OrderByDescending(p => p.Date)
            .ToList();

            var totalCount = products.Count;
            var subset = products.Skip((pageNumber - 1) * pageSize).Take(pageSize).ToList();
            var productsPaged = new StaticPagedList<ProductViewModel>(subset, pageNumber, pageSize, totalCount);

            // Cálculos
            decimal totalBalance = myAccounts.Sum(c => c.Balance);
            decimal totalDebt = myCards.Sum(t => t.CurrentDebt) + myLoans.Sum(p => p.Amount);
            decimal nextPayment = myLoans
                .Where(p => p.Installments != null && p.Installments.Any(i => !i.IsPaid))
                .SelectMany(p => p.Installments!)
                .Where(i => !i.IsPaid)
                .OrderBy(i => i.DueDate)
                .Select(i => i.PaymentAmount)
                .FirstOrDefault();

            // Próximas cuotas de préstamos
            decimal nextInstallments = myLoans
                .Where(p => p.Installments != null && p.Installments.Any(i => !i.IsPaid))
                .SelectMany(p => p.Installments!)
                .Where(i => !i.IsPaid)
                .OrderBy(i => i.DueDate)
                .Select(i => i.PaymentAmount)
                .FirstOrDefault();

            // Pago mínimo de tarjetas (generalmente 5% de la deuda)
            decimal minimumPayment = myCards
                .Where(t => t.CurrentDebt > 0)
                .Sum(t => t.CurrentDebt * 0.05m);

            ViewBag.TotalBalance = totalBalance.ToString("C2");
            ViewBag.TotalDebt = totalDebt.ToString("C2");
            ViewBag.NextPayment = nextPayment.ToString("C2");
            ViewBag.MinimumPayment = minimumPayment.ToString("C2");
            ViewBag.NextInstallments = nextInstallments.ToString("C2");

            return View(productsPaged);
        }

        [AllowAnonymous]
        public IActionResult AccessDenied()
        {
            return RedirectToAction("AccessDenied", "Auth");
        }
    }
}


/*
using System.Diagnostics;
using iii.Models;
using Microsoft.AspNetCore.Mvc;

namespace iii.Controllers
{
    public class HomeController : Controller
    {
        private readonly ILogger<HomeController> _logger;

        public HomeController(ILogger<HomeController> logger)
        {
            _logger = logger;
        }

        public IActionResult Index()
        {
            return View();
        }

        public IActionResult Privacy()
        {
            return View();
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }
}
*/