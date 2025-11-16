using ArtemisBanking.Core.Application.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace ArtemisBankingWebApp.Controllers
{
    [Authorize(Roles = "cliente")]
    public class TarjetasController : Controller
    {
        private readonly ICreditCardService _creditCardService;
        private readonly ITransactionService _transactionService;

        public TarjetasController(
            ICreditCardService creditCardService,
            ITransactionService transactionService)
        {
            _creditCardService = creditCardService;
            _transactionService = transactionService;
        }

        [HttpGet]
        public async Task<IActionResult> Detalle(int id)
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            var tarjeta = await _creditCardService.GetByIdAsync(id);

            // Verificar que la tarjeta pertenece al usuario autenticado
            if (tarjeta == null || tarjeta.UserId != userId)
                return RedirectToAction("Index", "Home");

            return View(tarjeta);
        }

        [HttpGet]
        public async Task<IActionResult> PagarTarjeta(int id)
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            var tarjeta = await _creditCardService.GetByIdAsync(id);

            // Verificar pertenencia
            if (tarjeta == null || tarjeta.UserId != userId)
                return RedirectToAction("Index", "Home");

            ViewBag.TarjetaId = id;
            ViewBag.DeudaActual = tarjeta.CurrentDebt;

            return View(tarjeta);
        }

        [HttpGet]
        public async Task<IActionResult> AvanceCash(int id)
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            var tarjeta = await _creditCardService.GetByIdAsync(id);

            // Verificar pertenencia
            if (tarjeta == null || tarjeta.UserId != userId)
                return RedirectToAction("Index", "Home");

            // Esta acción redirige al controlador de Avances
            return RedirectToAction("Index", "Avances", new { tarjetaId = id });
        }
    }
}
