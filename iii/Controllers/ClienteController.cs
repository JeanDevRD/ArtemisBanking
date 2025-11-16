using ArtemisBanking.Core.Application.Interfaces;
using ArtemisBanking.Core.Application.Dtos.CreditCard;
using ArtemisBanking.Core.Application.Dtos.Loan;
using ArtemisBanking.Core.Application.Dtos.SavingsAccount;
using ArtemisBanking.Core.Application.Dtos.Transaction;
using ArtemisBanking.Core.Application.ViewModels.Cliente;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace ArtemisBankingWebApp.Controllers
{
    [Authorize(Roles = "cliente")]
    public class ClienteController : Controller
    {
        private readonly ISavingsAccountService _savingsService;
        private readonly ILoanService _loanService;
        private readonly ICreditCardService _cardService;
        private readonly ITransactionService _transactionService;

        public ClienteController(
            ISavingsAccountService savingsService,
            ILoanService loanService,
            ICreditCardService cardService,
            ITransactionService transactionService)
        {
            _savingsService = savingsService;
            _loanService = loanService;
            _cardService = cardService;
            _transactionService = transactionService;
        }

        public async Task<IActionResult> Home()
        {
            var userId = User.FindFirstValue(System.Security.Claims.ClaimTypes.NameIdentifier);
            if (string.IsNullOrEmpty(userId))
                return RedirectToAction("Login", "Auth");

            var cuentas = await _savingsService.GetAllAsync();
            var prestamos = await _loanService.GetAllAsync();
            var tarjetas = await _cardService.GetAllAsync();
            var transacciones = await _transactionService.GetAllAsync();

            // Filtrar por usuario
            var misCuentas = cuentas.Where(c => c.UserId == userId).ToList();
            var misPrestamos = prestamos.Where(p => p.UserId == userId).ToList();
            var misTarjetas = tarjetas.Where(t => t.UserId == userId).ToList();

            var ultimas5 = transacciones
                .Where(t => t.SavingsAccount != null && t.SavingsAccount.UserId == userId)
                .OrderByDescending(t => t.Date)
                .Take(5)
                .ToList();

            decimal saldoTotal = misCuentas.Sum(c => c.Balance);
            decimal deudaTarjetas = misTarjetas.Sum(t => t.CurrentDebt);
            decimal deudaPrestamos = misPrestamos.Sum(p => p.Amount);
            decimal deudaTotal = deudaTarjetas + deudaPrestamos;

            decimal proximoPagoMinimo = misTarjetas
                .Where(t => t.CurrentDebt > 0)
                .Sum(t => t.CurrentDebt * 0.05m);

            decimal proximasCuotas = misPrestamos
                .Where(p => p.Installments != null && p.Installments.Any(i => !i.IsPaid))
                .SelectMany(p => p.Installments!)
                .Where(i => !i.IsPaid)
                .OrderBy(i => i.DueDate)
                .Select(i => i.PaymentAmount)
                .FirstOrDefault();

            var vm = new ClienteHomeViewModel
            {
                Cuentas = misCuentas,
                Prestamos = misPrestamos,
                Tarjetas = misTarjetas,
                UltimasTransacciones = ultimas5,
                SaldoTotal = saldoTotal,
                DeudaTotal = deudaTotal,
                ProximoPagoMinimo = proximoPagoMinimo,
                ProximasCuotas = proximasCuotas
            };

            return View(vm);
        }
    }
}

