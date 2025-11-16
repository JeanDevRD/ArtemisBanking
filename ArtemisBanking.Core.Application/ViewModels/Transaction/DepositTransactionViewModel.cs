namespace ArtemisBanking.Core.Application.ViewModels.Transaction
{
    public class DepositTransactionViewModel
    {
        public required string AccountNumber { get; set; }
        public required decimal Amount { get; set; }
    }
}
