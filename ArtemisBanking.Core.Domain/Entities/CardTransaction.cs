using ArtemisBanking.Core.Domain.Common;

namespace ArtemisBanking.Core.Domain.Entities
{
    public class CardTransaction : CommonEntity<int>
    {
        public DateTime Date { get; set; }
        public decimal Amount { get; set; }
        public required string Merchant { get; set; }
        public required string Status { get; set; } 
        public int CreditCardId { get; set; }
        public CreditCard CreditCard { get; set; } = null!;
    }
}
