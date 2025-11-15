// Models/DashboardViewModel.cs
using ArtemisBanking.Core.Application.Dtos.SavingsAccount;
using ArtemisBanking.Core.Application.Dtos.Loan;
using ArtemisBanking.Core.Application.Dtos.CreditCard;
using ArtemisBanking.Core.Application.Dtos.Transaction;

namespace ArtemisBankingWebApp.Models
{
    public class DashboardViewModel
    {
        public List<SavingsAccountDto> Cuentas { get; set; } = new();
        public List<LoanDto> Prestamos { get; set; } = new();
        public List<CreditCardDto> Tarjetas { get; set; } = new();
        public List<TransactionDto> UltimasTransacciones { get; set; } = new();
        public string SaldoTotal { get; set; } = "$0.00";
        public string DeudaTotal { get; set; } = "$0.00";
        public string ProximoPago { get; set; } = "$0.00";
    }
}