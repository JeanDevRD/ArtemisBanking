using ArtemisBanking.Core.Application.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace ArtemisBankingWebApp.Controllers
{
    [Authorize(Roles = "cliente")]
    public class CashAdvancesController : Controller
    {
        private readonly ICreditCardService _creditCardService;
        private readonly ITransactionService _transactionService;

        public CashAdvancesController(
            ICreditCardService creditCardService,
            ITransactionService transactionService)
        {
            _creditCardService = creditCardService;
            _transactionService = transactionService;
        }

        [HttpGet]
        public async Task<IActionResult> Index(int? cardId)
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);

            var cards = await _creditCardService.GetAllWithInclude();
            var myCards = cards.Where(t => t.UserId == userId && t.IsActive).ToList();

            if (cardId.HasValue)
            {
                var card = myCards.FirstOrDefault(t => t.Id == cardId.Value);
                if (card == null)
                    return RedirectToAction("Index", "Home");

                ViewBag.CardId = cardId.Value;
                ViewBag.CardNumber = $"****{card.CardNumber.Substring(card.CardNumber.Length - 4)}";
                ViewBag.AvailableLimit = card.CreditLimit - card.CurrentDebt;
                ViewBag.CreditLimit = card.CreditLimit;
            }

            ViewBag.Cards = myCards;

            return View();
        }
    }
}
