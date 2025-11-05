using ArtemisBanking.Core.Domain.Common;
using System.Transactions;

namespace ArtemisBanking.Core.Domain.Entities
{
    public class SavingsAccount : CommonEntity<int>
    {
        public required string AccountNumber { get; set; }
        public decimal Balance { get; set; }
        public required string Type { get; set; } 
        public bool IsActive { get; set; } = true;
        public DateTime CreatedAt { get; set; } = DateTime.Now;
        public required string UserId { get; set; } 
        public ICollection<Transaction>? Transactions { get; set; }
    }
}
