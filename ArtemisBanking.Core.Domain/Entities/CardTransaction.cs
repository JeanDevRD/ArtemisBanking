using ArtemisBanking.Core.Domain.Common;

namespace ArtemisBanking.Core.Domain.Entities
{
    //Registra cada consumo o intento de consumo realizado con una tarjeta de crédito,
    //incluyendo su estado de aprobación o rechazo.
    public class CardTransaction : CommonEntity<int>
    {
        public DateTime Date { get; set; }
        public decimal Amount { get; set; }
        public required string Merchant { get; set; }
        public required int Status { get; set; } 
        public int CreditCardId { get; set; }
        public CreditCard CreditCard { get; set; } = null!;
    }
}
