namespace ArtemisBanking.Core.Application.ViewModels.Transaction
{
    public class ProcessPaymentViewModel
    {
        public required string CardNumber { get; set; }
        public required string MonthExpirationCard { get; set; }
        public required string YearExpirationCard { get; set; }
        public required string CVC { get; set; }
        public decimal TransactionAmount { get; set; }
    }
}
