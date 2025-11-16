namespace ArtemisBanking.Core.Application.ViewModels.Transaction
{
    public class HomeCasherViewModel
    {
        public CountTransactionViewModel TotalTransaction { get; set; } = new();
        public CountDepositViewModel TotalDeposit { get; set; } = new();
        public CountWithdrawalViewModel TotalWithdrawal { get; set; } = new();
        public CountPaidsViewModel TotalPaids { get; set; } = new();
    }
}
