using ArtemisBanking.Core.Application.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace ArtemisBankingWebApp.Controllers
{
    [Authorize(Roles = "cliente")]
    public class PrestamosController : Controller
    {
        private readonly ILoanService _loanService;
        private readonly ITransactionService _transactionService;

        public PrestamosController(
            ILoanService loanService,
            ITransactionService transactionService)
        {
            _loanService = loanService;
            _transactionService = transactionService;
        }

        [HttpGet]
        public async Task<IActionResult> Detalle(int id)
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            var prestamo = await _loanService.GetByIdAsync(id);

            // Verificar que el préstamo pertenece al usuario autenticado
            if (prestamo == null || prestamo.UserId != userId)
                return RedirectToAction("Index", "Home");

            return View(prestamo);
        }

        [HttpGet]
        public async Task<IActionResult> PagarCuota(int id)
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            var prestamo = await _loanService.GetByIdAsync(id);

            // Verificar pertenencia
            if (prestamo == null || prestamo.UserId != userId)
                return RedirectToAction("Index", "Home");

            // Obtener cuota pendiente
            var cuotaPendiente = prestamo.Installments?
                .FirstOrDefault(i => !i.IsPaid);

            if (cuotaPendiente == null)
            {
                TempData["Info"] = "No hay cuotas pendientes de pago.";
                return RedirectToAction("Detalle", new { id });
            }

            ViewBag.PrestamoId = id;
            ViewBag.MontoCuota = cuotaPendiente.PaymentAmount;
            ViewBag.FechaVencimiento = cuotaPendiente.DueDate;

            return View(prestamo);
        }
    }
}
