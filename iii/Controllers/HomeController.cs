using ArtemisBanking.Core.Application.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace ArtemisBankingWebApp.Controllers
{
    [Authorize(Roles = "Cliente")]
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

        public async Task<IActionResult> Index()
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            var cuentas = await _savingsAccountService.GetAllAsync();
            var prestamos = await _loanService.GetAllAsync();
            var tarjetas = await _creditCardService.GetAllAsync();
            var transacciones = await _transactionService.GetAllAsync();

            var misCuentas = cuentas.Where(c => c.UserId == userId).ToList();
            var misPrestamos = prestamos.Where(p => p.UserId == userId).ToList();
            var misTarjetas = tarjetas.Where(t => t.UserId == userId).ToList();
            var misTransacciones = transacciones
                .Where(t => t.SavingsAccount?.UserId == userId)
                .OrderByDescending(t => t.Date)
                .Take(5)
                .ToList();

            // Calcular totales
            decimal saldoTotal = misCuentas.Sum(c => c.Balance);
            decimal deudaTotal = misTarjetas.Sum(t => t.CurrentDebt) + misPrestamos.Sum(p => p.Amount);
            decimal proximoPagoMinimo = misPrestamos
                .Sum(p => p.Installments?.FirstOrDefault(i => !i.IsPaid)?.Amount ?? 0);

            ViewBag.SaldoTotal = saldoTotal.ToString("C2");
            ViewBag.DeudaTotal = deudaTotal.ToString("C2");
            ViewBag.ProximoPago = proximoPagoMinimo.ToString("C2");
            ViewBag.Transacciones = misTransacciones;

            var productos = new
            {
                Cuentas = misCuentas,
                Prestamos = misPrestamos,
                Tarjetas = misTarjetas
            };

            return View(productos);
        }

        // Método para acceso denegado - permite acceso anónimo
        [AllowAnonymous]
        public IActionResult AccesoDenegado()
        {
            return RedirectToAction("AccesoDenegado", "Auth");
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