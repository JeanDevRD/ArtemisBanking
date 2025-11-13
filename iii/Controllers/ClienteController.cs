using ArtemisBanking.Core.Application.Interfaces;
using ArtemisBanking.Core.Application.Dtos.CreditCard;
using ArtemisBanking.Core.Application.Dtos.Loan;
using ArtemisBanking.Core.Application.Dtos.SavingsAccount;
using ArtemisBanking.Core.Application.Dtos.Transaction;
using ArtemisBanking.Core.Application.ViewModels.Cliente;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

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
            var cuentas = await _savingsService.GetAllAsync();
            var prestamos = await _loanService.GetAllAsync();
            var tarjetas = await _cardService.GetAllAsync();
            var transacciones = await _transactionService.GetAllAsync();

            var ultimas5 = transacciones
                .OrderByDescending(t => t.Date)
                .Take(5)
                .ToList();

            decimal saldoTotal = cuentas.Sum(c => c.Balance);
            decimal deudaTotal = prestamos.Sum(p => p.Amount);

            //  
          //  decimal proximoPagoMinimo = tarjetas.Sum(t => t.MinimumPayment); //

            // 
            decimal proximasCuotas = prestamos.Sum(p =>
                p.Installments.Any() ? p.Installments.First().PaymentAmount : 0);

            var vm = new ClienteHomeViewModel
            {
                Cuentas = cuentas,
                Prestamos = prestamos,
                Tarjetas = tarjetas,
                UltimasTransacciones = ultimas5,
                SaldoTotal = saldoTotal,
                DeudaTotal = deudaTotal,
               // ProximoPagoMinimo = proximoPagoMinimo,
                ProximasCuotas = proximasCuotas
            };

            return View(vm);
        }
    }
}

