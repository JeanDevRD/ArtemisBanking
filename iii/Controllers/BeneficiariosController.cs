using ArtemisBanking.Core.Application.Dtos.Beneficiary;
using ArtemisBanking.Core.Application.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace ArtemisBankingWebApp.Controllers
{
    [Authorize(Roles = "Cliente")]
    public class BeneficiariosController : Controller
    {
        private readonly IBeneficiaryService _beneficiaryService;
        private readonly ITransactionService _transactionService;

        public BeneficiariosController(
            IBeneficiaryService beneficiaryService,
            ITransactionService transactionService)
        {
            _beneficiaryService = beneficiaryService;
            _transactionService = transactionService;
        }

        public async Task<IActionResult> Index()
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            var beneficiarios = await _beneficiaryService.GetAllAsync();
            var list = beneficiarios
                .Where(b => b.UserId == userId)
                .OrderBy(b => b.FirstName)
                .Take(20)
                .ToList();

            return View(list);
        }

        [HttpGet]
        public IActionResult Agregar() => View();

        [HttpPost]
        public async Task<IActionResult> Agregar(BeneficiaryDto model)
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);

            if (!ModelState.IsValid)
                return View(model);

            // Validar número de cuenta (9 dígitos)
            if (model.AccountNumber.Length != 9 || !model.AccountNumber.All(char.IsDigit))
            {
                ModelState.AddModelError("", "El número de cuenta debe tener 9 dígitos numéricos.");
                return View(model);
            }

            // Validar cédula (no puede ser la del propio usuario)
            if (await _beneficiaryService.ExistsByCedulaAsync(model.UserId))
            {
                ModelState.AddModelError("", "No puedes registrar tu propia cédula como beneficiario.");
                return View(model);
            }

            // Validar cuenta duplicada
            if (await _beneficiaryService.ExistsByAccountNumberAsync(model.AccountNumber))
            {
                ModelState.AddModelError("", "Ya existe un beneficiario con ese número de cuenta.");
                return View(model);
            }

            model.UserId = userId;
            await _beneficiaryService.AddAsync(model);

            TempData["Success"] = "Beneficiario agregado correctamente.";
            return RedirectToAction(nameof(Index));
        }

        [HttpGet]
        public async Task<IActionResult> Editar(int id)
        {
            var beneficiary = await _beneficiaryService.GetByIdAsync(id);
            return View(beneficiary);
        }

        [HttpPost]
        public async Task<IActionResult> Editar(BeneficiaryDto model)
        {
            if (!ModelState.IsValid)
                return View(model);

            await _beneficiaryService.UpdateAsync(model, model.Id);
            TempData["Success"] = "Beneficiario actualizado correctamente.";
            return RedirectToAction(nameof(Index));
        }

        [HttpGet]
        public async Task<IActionResult> Eliminar(int id)
        {
            var tieneTransacciones = await _beneficiaryService.HasPendingTransactionsAsync(id);

            if (tieneTransacciones)
            {
                TempData["Error"] = "No puedes eliminar un beneficiario con transacciones pendientes.";
                return RedirectToAction(nameof(Index));
            }

            await _beneficiaryService.DeleteAsync(id);
            TempData["Success"] = "Beneficiario eliminado correctamente.";
            return RedirectToAction(nameof(Index));
        }
    }
}



