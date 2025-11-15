using ArtemisBanking.Core.Application.Interfaces;
using ArtemisBankingWebApp.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;
using X.PagedList;
using X.PagedList.Mvc.Core;

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

        public async Task<IActionResult> Index(int? page)
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            if (string.IsNullOrEmpty(userId))
                return RedirectToAction("Login", "Auth");

            var pageNumber = page ?? 1;
            var pageSize = 20;

            var cuentas = await _savingsAccountService.GetAllAsync();
            var prestamos = await _loanService.GetAllAsync();
            var tarjetas = await _creditCardService.GetAllAsync();

            var misCuentas = cuentas.Where(c => c.UserId == userId).ToList();
            var misPrestamos = prestamos.Where(p => p.UserId == userId).ToList();
            var misTarjetas = tarjetas.Where(t => t.UserId == userId).ToList();

            // ORDEN: más reciente → antiguo
            var productos = misCuentas.Select(c => new ProductoViewModel
            {
                Tipo = "Cuenta de Ahorro",
                Numero = c.AccountNumber,
                Monto = c.Balance,
                Estado = c.IsActive ? "Activa" : "Inactiva",
                Fecha = c.CreatedAt,
                Id = c.Id,
                TipoId = 1
            })
            .Concat(misPrestamos.Select(p => new ProductoViewModel
            {
                Tipo = "Préstamo",
                Numero = p.LoanNumber,
                Monto = p.Amount,
                Estado = p.IsActive ? "Activo" : "Cerrado",
                Fecha = p.ApprovedAt,
                Id = p.Id,
                TipoId = 2
            }))
            .Concat(misTarjetas.Select(t => new ProductoViewModel
            {
                Tipo = "Tarjeta de Crédito",
                Numero = t.CardNumber,
                Monto = t.CurrentDebt,
                Estado = t.IsActive ? "Activa" : "Inactiva",
                Fecha = t.CreateAt,
                Id = t.Id,
                TipoId = 3
            }))
            .OrderByDescending(p => p.Fecha)
            .ToList();

            var totalCount = productos.Count;
            var subset = productos.Skip((pageNumber - 1) * pageSize).Take(pageSize).ToList();
            var productosPaged = new StaticPagedList<ProductoViewModel>(subset, pageNumber, pageSize, totalCount);

            // Cálculos
            decimal saldoTotal = misCuentas.Sum(c => c.Balance);
            decimal deudaTotal = misTarjetas.Sum(t => t.CurrentDebt) + misPrestamos.Sum(p => p.Amount);
            decimal proximoPago = misPrestamos
                .Where(p => p.Installments != null)
                .Sum(p => p.Installments.Where(i => !i.IsPaid).OrderBy(i => i.DueDate).FirstOrDefault()?.Amount ?? 0);

            ViewBag.SaldoTotal = saldoTotal.ToString("C2");
            ViewBag.DeudaTotal = deudaTotal.ToString("C2");
            ViewBag.ProximoPago = proximoPago.ToString("C2");

            return View(productosPaged);
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