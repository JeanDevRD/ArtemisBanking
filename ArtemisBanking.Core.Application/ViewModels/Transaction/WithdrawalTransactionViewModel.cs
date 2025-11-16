namespace ArtemisBanking.Core.Application.ViewModels.Transaction
{
    public class WithdrawalTransactionViewModel
    {
        public required string AccountNumber { get; set; }
        public required decimal Amount { get; set; }
    }
}
