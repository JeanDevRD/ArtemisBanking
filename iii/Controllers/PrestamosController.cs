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
        public async Task<IActionResult> Detail(int id)
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            var loan = await _loanService.GetByIdAsync(id);

            if (loan == null || loan.UserId != userId)
                return RedirectToAction("Index", "Home");

            // Thin adapter: redirect to canonical Loan controller
            return RedirectToAction("Detail", "Loan", new { loanId = id });
        }

        [HttpGet]
        public async Task<IActionResult> PayInstallment(int id)
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            var loan = await _loanService.GetByIdAsync(id);

            if (loan == null || loan.UserId != userId)
                return RedirectToAction("Index", "Home");

            // Thin adapter: redirect to canonical Loan controller with payment flag
            return RedirectToAction("Detail", "Loan", new { loanId = id, payInstallment = true });
        }
    }
}
