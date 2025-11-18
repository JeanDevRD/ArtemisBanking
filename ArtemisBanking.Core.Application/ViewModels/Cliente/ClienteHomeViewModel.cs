using ArtemisBanking.Core.Application.Dtos.CreditCard;
using ArtemisBanking.Core.Application.Dtos.Loan;
using ArtemisBanking.Core.Application.Dtos.SavingsAccount;
using ArtemisBanking.Core.Application.Dtos.Transaction;
using System.Collections.Generic;

namespace ArtemisBanking.Core.Application.ViewModels.Cliente
{
    public class ClientHomeViewModel
    {
        public List<SavingsAccountDto> Accounts { get; set; } = new();
        public List<LoanDto> Loans { get; set; } = new();
        public List<CreditCardDto> Cards { get; set; } = new();
        public List<TransactionDto> LastTransactions { get; set; } = new();

        public decimal TotalBalance { get; set; }
        public decimal TotalDebt { get; set; }
        public decimal MinimumPayment { get; set; }
        public decimal NextInstallments { get; set; }
    }
}
