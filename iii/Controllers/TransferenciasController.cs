using ArtemisBanking.Core.Application.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace ArtemisBankingWebApp.Controllers
{
    [Authorize(Roles = "cliente")]
    public class TransferenciasController : Controller
    {
        private readonly ISavingsAccountService _savingsAccountService;
        private readonly IBeneficiaryService _beneficiaryService;
        private readonly ITransactionService _transactionService;

        public TransferenciasController(
            ISavingsAccountService savingsAccountService,
            IBeneficiaryService beneficiaryService,
            ITransactionService transactionService)
        {
            _savingsAccountService = savingsAccountService;
            _beneficiaryService = beneficiaryService;
            _transactionService = transactionService;
        }

        [HttpGet]
        public async Task<IActionResult> Index(int? cuentaId)
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);

            // Obtener cuentas del usuario
            var cuentas = await _savingsAccountService.GetAllAsync();
            var misCuentas = cuentas.Where(c => c.UserId == userId).ToList();

            // Si viene cuentaId, verificar que exista y pertenezca al usuario
            if (cuentaId.HasValue)
            {
                var cuenta = misCuentas.FirstOrDefault(c => c.Id == cuentaId.Value);
                if (cuenta == null)
                    return RedirectToAction("Index", "Home");

                ViewBag.CuentaOrigenId = cuentaId.Value;
                ViewBag.CuentaOrigenNumero = cuenta.AccountNumber;
                ViewBag.SaldoDisponible = cuenta.Balance;
            }

            // Obtener beneficiarios del usuario
            var beneficiarios = await _beneficiaryService.GetAllAsync();
            var misBeneficiarios = beneficiarios.Where(b => b.UserId == userId).ToList();

            ViewBag.Cuentas = misCuentas;
            ViewBag.Beneficiarios = misBeneficiarios;

            return View();
        }
    }
}
