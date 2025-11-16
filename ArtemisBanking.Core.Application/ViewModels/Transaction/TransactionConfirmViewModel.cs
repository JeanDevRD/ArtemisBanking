namespace ArtemisBanking.Core.Application.ViewModels.Transaction
{
    public class TransactionConfirmViewModel
    {
        public int AccountId { get; set; }
        public required string AccountNumber { get; set; }
        public required string HolderName { get; set; }
        public decimal Amount { get; set; }
    }
}
