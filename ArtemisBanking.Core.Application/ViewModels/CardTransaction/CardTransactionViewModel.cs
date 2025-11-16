using ArtemisBanking.Core.Application.Dtos.CreditCard;
using ArtemisBanking.Core.Application.ViewModels.Common;


namespace ArtemisBanking.Core.Application.Dtos.CardTransaction
{
    public class CardTransactionViewModel : CommonEntityViewModel<int>
    {
        public DateTime Date { get; set; }
        public decimal Amount { get; set; }
        public required string Merchant { get; set; }
        public required int Status { get; set; }
        public int CreditCardId { get; set; }
        public CreditCardDto CreditCard { get; set; } = null!;
    }
}
