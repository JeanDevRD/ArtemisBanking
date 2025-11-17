using ArtemisBanking.Core.Application.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace ArtemisBankingWebApp.Controllers
{
    [Authorize(Roles = "cliente")]
    public class CardsController : Controller
    {
        private readonly ICreditCardService _creditCardService;
        private readonly ITransactionService _transactionService;

        public CardsController(
            ICreditCardService creditCardService,
            ITransactionService transactionService)
        {
            _creditCardService = creditCardService;
            _transactionService = transactionService;
        }

        [HttpGet]
        public async Task<IActionResult> Detail(int id)
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            var card = await _creditCardService.GetByIdAsync(id);

            // Verificar que la tarjeta pertenece al usuario autenticado
            if (card == null || card.UserId != userId)
                return RedirectToAction("Index", "Home");

            return View(card);
        }

        [HttpGet]
        public async Task<IActionResult> PayCard(int id)
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            var card = await _creditCardService.GetByIdAsync(id);

            // Verificar pertenencia
            if (card == null || card.UserId != userId)
                return RedirectToAction("Index", "Home");

            ViewBag.CardId = id;
            ViewBag.CurrentDebt = card.CurrentDebt;

            return View(card);
        }

        [HttpGet]
        public async Task<IActionResult> CashAdvance(int id)
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            var card = await _creditCardService.GetByIdAsync(id);

            // Verificar pertenencia
            if (card == null || card.UserId != userId)
                return RedirectToAction("Index", "Home");

            // Esta acción redirige al controlador de Avances
            return RedirectToAction("Index", "CashAdvances", new { cardId = id });
        }
    }
}
