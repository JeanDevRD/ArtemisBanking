using ArtemisBanking.Core.Domain.Common;

namespace ArtemisBanking.Core.Domain.Entities
{
    public class Transaction : CommonEntity<int>
    {
        public DateTime Date { get; set; }
        public decimal Amount { get; set; }
        public required string Type { get; set; } 
        public required string Source { get; set; }
        public required string Beneficiary { get; set; }
        public required string Status { get; set; } 
        public int SavingsAccountId { get; set; }
        public SavingsAccount SavingsAccount { get; set; } = null!;
    }
}
