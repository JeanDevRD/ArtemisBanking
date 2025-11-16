using ArtemisBanking.Core.Application.Dtos.CreditCard;
using ArtemisBanking.Core.Application.Interfaces;
using ArtemisBanking.Core.Application.ViewModel.CreditCard;
using ArtemisBanking.Core.Application.ViewModels.CreditCard;
using AutoMapper;
using Microsoft.AspNetCore.Mvc;

namespace ArtemisBankingWebApp.Controllers
{
    public class CreditCardController : Controller
    {
        private readonly ICreditCardService _creditCardService;
        private readonly IMapper _mapper;

        public CreditCardController(ICreditCardService creditCardService, IMapper mapper)
        {
            _creditCardService = creditCardService;
            _mapper = mapper;
        }

        public async Task<IActionResult> Index(int pageNumber = 1, int pageSize = 20, string? identificationNumber = null, bool? isActive = null)
        {
            var result = string.IsNullOrEmpty(identificationNumber)
                ? await _creditCardService.GetAllActiveCreditCard()
                : await _creditCardService.GetAllCreditCardByIdentityUser(identificationNumber, isActive);

            if (result.IsError || result.Result == null)
            {
                TempData["Error"] = result.Message ?? "No se encontraron tarjetas";
                return View("Index", new HomeCreditCardViewModel());
            }

            var data = new HomeCreditCardViewModel
            {
                Cards = _mapper.Map<List<CreditCardListViewModel>>(result.Result),
                PageNumber = pageNumber,
                TotalPages = (int)Math.Ceiling((double)result.Result.Count / pageSize),
                FilterIdentificationNumber = identificationNumber,
                IsActive = isActive
            };

            return View("Index", data);
        }

        public IActionResult Create(string userId)
        {
            return View("Create", new CreditCardRequestViewModel
            {
                IdCliente = userId,
                CreditLimit = 0
            });
        }

        [HttpPost]
        public async Task<IActionResult> Create(CreditCardRequestViewModel vm)
        {
            if (!ModelState.IsValid)
                return View("Create", vm);

            var dto = _mapper.Map<CreditCardRequestDto>(vm);
            var result = await _creditCardService.AddCreditCardAsync(dto, "ADMIN_ID");

            if (result.IsError)
            {
                ModelState.AddModelError(string.Empty, result.Message ?? "Error al asignar tarjeta");
                return View("Create", vm);
            }

            TempData["Success"] = $"Tarjeta creada exitosamente para el cliente {vm.IdCliente}";
            return RedirectToAction("Index");
        }

        public async Task<IActionResult> Detail(int cardId)
        {
            var result = await _creditCardService.DetailCard(cardId);

            if (result.IsError || result.Result == null)
            {
                TempData["Error"] = result.Message ?? "Tarjeta no encontrada";
                return RedirectToAction("Index");
            }

            var vm = _mapper.Map<CreditCardDetailViewModel>(result.Result);
            return View("Detail", vm);
        }

        public async Task<IActionResult> Edit(int cardId)
        {
            var card = await _creditCardService.GetByIdAsync(cardId);
            if (card == null)
                return RedirectToAction("Index");

            var vm = new UpdateCreditCardViewModel { NewCreditLimit = card.CreditLimit };
            ViewBag.CardId = cardId;
            return View("Edit", vm);
        }

        [HttpPost]
        public async Task<IActionResult> Edit(int cardId, UpdateCreditCardViewModel vm)
        {
            if (!ModelState.IsValid)
                return View("Edit", vm);

            var dto = _mapper.Map<UpdateCreditCardDto>(vm);
            var result = await _creditCardService.UpdateCard(dto, cardId);

            if (result.IsError)
            {
                ModelState.AddModelError(string.Empty, result.Message ?? "Error al actualizar tarjeta");
                return View("Edit", vm);
            }

            TempData["Success"] = $"Tarjeta #{result.Result.CardNumber} actualizada correctamente.";
            return RedirectToAction("Index");
        }

        public async Task<IActionResult> Cancel(int cardId)
        {
            var card = await _creditCardService.GetByIdAsync(cardId);
            if (card == null)
                return RedirectToAction("Index");

            var vm = new CancelCreditCardViewModel
            {
                CardId = cardId,
                LastDigits = card.CardNumber.Substring(card.CardNumber.Length - 4),
                CanCancel = card.CurrentDebt == 0
            };

            return View("Cancel", vm);
        }

        [HttpPost]
        public async Task<IActionResult> CancelConfirmed(int cardId)
        {
            var success = await _creditCardService.CancelatedCreditCard(cardId);

            if (!success)
            {
                TempData["Error"] = "Para cancelar esta tarjeta, el cliente debe saldar la deuda pendiente.";
                return RedirectToAction("Index");
            }

            TempData["Success"] = "Tarjeta cancelada exitosamente.";
            return RedirectToAction("Index");
        }
    }
}
