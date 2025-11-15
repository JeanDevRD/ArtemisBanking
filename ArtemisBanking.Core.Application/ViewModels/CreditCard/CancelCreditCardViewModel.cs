namespace ArtemisBanking.Core.Application.ViewModels.CreditCard
{
    public class CancelCreditCardViewModel
    {
        public int CardId { get; set; }
        public string LastDigits { get; set; } = "";
        public bool CanCancel { get; set; }
    }
}
