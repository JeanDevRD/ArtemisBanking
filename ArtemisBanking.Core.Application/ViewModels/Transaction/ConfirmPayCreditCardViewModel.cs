namespace ArtemisBanking.Core.Application.ViewModels.Transaction
{
    public class ConfirmPayCreditCardViewModel
    {
        public required string AccountNumber { get; set; }
        public decimal Amount { get; set; }
        public required string CardNumber { get; set; }
        public required string CardHolderName { get; set; }
        public required string LastFourDigits { get; set; }
        public decimal CurrentDebt { get; set; }
    }
}

