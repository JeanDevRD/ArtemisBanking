using ArtemisBanking.Core.Application.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace ArtemisBankingWebApp.Controllers
{
    [Authorize(Roles = "cliente")]
    public class CuentasController : Controller
    {
        private readonly ISavingsAccountService _savingsAccountService;
        private readonly ITransactionService _transactionService;

        public CuentasController(
            ISavingsAccountService savingsAccountService,
            ITransactionService transactionService)
        {
            _savingsAccountService = savingsAccountService;
            _transactionService = transactionService;
        }

        [HttpGet]
        public async Task<IActionResult> Detalle(int id)
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            var cuenta = await _savingsAccountService.GetByIdAsync(id);

            // Verificar que la cuenta pertenece al usuario autenticado
            if (cuenta == null || cuenta.UserId != userId)
                return RedirectToAction("Index", "Home");

            // Obtener últimas transacciones de esta cuenta
            var transacciones = await _transactionService.GetAllWithInclude();
            var transactionesDelaCuenta = transacciones
                .Where(t => t.SavingsAccountId == id)
                .OrderByDescending(t => t.Date)
                .Take(10)
                .ToList();

            ViewBag.Transacciones = transactionesDelaCuenta;

            return View(cuenta);
        }

        [HttpGet]
        public async Task<IActionResult> Transferir(int id)
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            var cuenta = await _savingsAccountService.GetByIdAsync(id);

            // Verificar pertenencia
            if (cuenta == null || cuenta.UserId != userId)
                return RedirectToAction("Index", "Home");

            // Esta acción redirige al controlador de Transferencias
            return RedirectToAction("Index", "Transferencias", new { cuentaId = id });
        }

        [HttpGet]
        public async Task<IActionResult> Depositar(int id)
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            var cuenta = await _savingsAccountService.GetByIdAsync(id);

            // Verificar pertenencia
            if (cuenta == null || cuenta.UserId != userId)
                return RedirectToAction("Index", "Home");

            ViewBag.CuentaId = id;
            ViewBag.CuentaNumero = cuenta.AccountNumber;

            return View(cuenta);
        }

        [HttpGet]
        public async Task<IActionResult> Retirar(int id)
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            var cuenta = await _savingsAccountService.GetByIdAsync(id);

            // Verificar pertenencia
            if (cuenta == null || cuenta.UserId != userId)
                return RedirectToAction("Index", "Home");

            ViewBag.CuentaId = id;
            ViewBag.CuentaNumero = cuenta.AccountNumber;

            return View(cuenta);
        }
    }
}
