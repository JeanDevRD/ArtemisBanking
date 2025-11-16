using ArtemisBanking.Core.Application.Dtos.Common;
using ArtemisBanking.Core.Application.Dtos.SavingsAccount;
using ArtemisBanking.Core.Application.ViewModel.SavingsAccount;
using ArtemisBanking.Core.Application.ViewModels.Common;

namespace ArtemisBanking.Core.Application.ViewModels.Transaction
{
    public class TransactionViewModel : CommonEntityViewModel<int>
    {
        public DateTime Date { get; set; }
        public decimal Amount { get; set; }
        public required int Type { get; set; } 
        public required string Source { get; set; }
        public required string Beneficiary { get; set; }
        public required int Status { get; set; } 
        public int SavingsAccountId { get; set; }
        public required string CashierId { get; set; }

        public string? ComprobanteUrl { get; set; } // ← NUEVO
        public SavingsAccountViewModel SavingsAccount { get; set; } = null!;
    }
}
