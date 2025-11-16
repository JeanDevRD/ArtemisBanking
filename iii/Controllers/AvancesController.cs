using ArtemisBanking.Core.Application.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace ArtemisBankingWebApp.Controllers
{
    [Authorize(Roles = "cliente")]
    public class AvancesController : Controller
    {
        private readonly ICreditCardService _creditCardService;
        private readonly ITransactionService _transactionService;

        public AvancesController(
            ICreditCardService creditCardService,
            ITransactionService transactionService)
        {
            _creditCardService = creditCardService;
            _transactionService = transactionService;
        }

        [HttpGet]
        public async Task<IActionResult> Index(int? tarjetaId)
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);

            // Obtener tarjetas del usuario
            var tarjetas = await _creditCardService.GetAllWithInclude();
            var misTarjetas = tarjetas.Where(t => t.UserId == userId && t.IsActive).ToList();

            // Si viene tarjetaId, verificar que exista y pertenezca al usuario
            if (tarjetaId.HasValue)
            {
                var tarjeta = misTarjetas.FirstOrDefault(t => t.Id == tarjetaId.Value);
                if (tarjeta == null)
                    return RedirectToAction("Index", "Home");

                ViewBag.TarjetaId = tarjetaId.Value;
                ViewBag.TarjetaNumero = $"****{tarjeta.CardNumber.Substring(tarjeta.CardNumber.Length - 4)}";
                ViewBag.LimiteDisponible = tarjeta.CreditLimit - tarjeta.CurrentDebt;
                ViewBag.CreditoDisponible = tarjeta.CreditLimit;
            }

            ViewBag.Tarjetas = misTarjetas;

            return View();
        }
    }
}
