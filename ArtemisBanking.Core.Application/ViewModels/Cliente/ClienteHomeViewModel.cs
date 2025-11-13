using ArtemisBanking.Core.Application.Dtos.CreditCard;
using ArtemisBanking.Core.Application.Dtos.Loan;
using ArtemisBanking.Core.Application.Dtos.SavingsAccount;
using ArtemisBanking.Core.Application.Dtos.Transaction;
using System.Collections.Generic;

namespace ArtemisBanking.Core.Application.ViewModels.Cliente
{
    public class ClienteHomeViewModel
    {
        public List<SavingsAccountDto> Cuentas { get; set; } = new();
        public List<LoanDto> Prestamos { get; set; } = new();
        public List<CreditCardDto> Tarjetas { get; set; } = new();
        public List<TransactionDto> UltimasTransacciones { get; set; } = new();

        public decimal SaldoTotal { get; set; }
        public decimal DeudaTotal { get; set; }
        public decimal ProximoPagoMinimo { get; set; }
        public decimal ProximasCuotas { get; set; }
    }
}
