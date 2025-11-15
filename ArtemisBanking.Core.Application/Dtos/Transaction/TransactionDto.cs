using ArtemisBanking.Core.Application.Dtos.Common;
using ArtemisBanking.Core.Application.Dtos.SavingsAccount;

namespace ArtemisBanking.Core.Application.Dtos.Transaction
{
    public class TransactionDto : CommonEntityDto<int>
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
        public SavingsAccountDto SavingsAccount { get; set; } = null!;
    }
}
