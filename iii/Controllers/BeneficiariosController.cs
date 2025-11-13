using ArtemisBanking.Core.Application.Dtos.Beneficiary;
using ArtemisBanking.Core.Application.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace ArtemisBankingWebApp.Controllers
{
    [Authorize(Roles = "cliente")]
    public class BeneficiariosController : Controller
    {
        private readonly IBeneficiaryService _beneficiaryService;

        public BeneficiariosController(IBeneficiaryService beneficiaryService)
        {
            _beneficiaryService = beneficiaryService;
        }

        public async Task<IActionResult> Index()
        {
            var list = await _beneficiaryService.GetAllAsync();
            return View(list.OrderBy(b => b.FirstName).Take(20).ToList());
        }

        public IActionResult Agregar() => View();

        [HttpPost]
        public async Task<IActionResult> Agregar(BeneficiaryDto model)
        {
            if (!ModelState.IsValid)
                return View(model);

            if (await _beneficiaryService.ExistsByAccountNumberAsync(model.AccountNumber))
            {
                ModelState.AddModelError("", "Ya existe un beneficiario con ese número de cuenta.");
                return View(model);
            }

            if (await _beneficiaryService.ExistsByCedulaAsync(model.UserId))
            {
                ModelState.AddModelError("", "No puedes agregar tu propia cédula como beneficiario.");
                return View(model);
            }

            await _beneficiaryService.AddAsync(model);
            return RedirectToAction(nameof(Index));
        }

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

            await _beneficiaryService.UpdateAsync(model.Id, model);

            return RedirectToAction(nameof(Index));
        }

        public async Task<IActionResult> Eliminar(int id)
        {
            if (await _beneficiaryService.HasPendingTransactionsAsync(id))
            {
                TempData["Error"] = "No puedes eliminar un beneficiario con transacciones pendientes.";
                return RedirectToAction(nameof(Index));
            }

            await _beneficiaryService.DeleteAsync(id);
            return RedirectToAction(nameof(Index));
        }
    }
}


