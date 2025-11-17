using ArtemisBanking.Core.Application.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace ArtemisBankingWebApp.Controllers
{
    [Authorize(Roles = "cliente")]
    public class LoansController : Controller
    {
        private readonly ILoanService _loanService;
        private readonly ITransactionService _transactionService;

        public LoansController(
            ILoanService loanService,
            ITransactionService transactionService)
        {
            _loanService = loanService;
            _transactionService = transactionService;
        }

        [HttpGet]
        public async Task<IActionResult> Detail(int id)
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            var loan = await _loanService.GetByIdAsync(id);

            // Verificar que el préstamo pertenece al usuario autenticado
            if (loan == null || loan.UserId != userId)
                return RedirectToAction("Index", "Home");

            return View(loan);
        }

        [HttpGet]
        public async Task<IActionResult> PayInstallment(int id)
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            var loan = await _loanService.GetByIdAsync(id);

            // Verificar pertenencia
            if (loan == null || loan.UserId != userId)
                return RedirectToAction("Index", "Home");

            // Obtener cuota pendiente
            var pendingInstallment = loan.Installments?
                .FirstOrDefault(i => !i.IsPaid);

            if (pendingInstallment == null)
            {
                TempData["Info"] = "No hay cuotas pendientes de pago.";
                return RedirectToAction("Detail", new { id });
            }

            ViewBag.LoanId = id;
            ViewBag.InstallmentAmount = pendingInstallment.PaymentAmount;
            ViewBag.DueDate = pendingInstallment.DueDate;

            return View(loan);
        }
    }
}
