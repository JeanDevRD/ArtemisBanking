using ArtemisBanking.Core.Domain.Common;

namespace ArtemisBanking.Core.Domain.Entities
{
    public class CreditCard : CommonEntity<int>
    {
        public required string CardNumber { get; set; }
        public required string CvcHash { get; set; }
        public decimal CreditLimit { get; set; }
        public decimal CurrentDebt { get; set; }
        public DateTime ExpirationDate { get; set; }
        public bool IsActive { get; set; } = true;
        public required string UserId { get; set; }
        public ICollection<CardTransaction>? CardTransactions { get; set; }
    }
}
