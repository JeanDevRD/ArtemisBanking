namespace ArtemisBanking.Core.Application.ViewModels.SavingsAccount
{
    public class CancelSavingsAccountViewModel
    {
        public int AccountId { get; set; }
        public string AccountNumber { get; set; } = "";
        public bool IsSecondary { get; set; }
        public bool CanCancel { get; set; }
    }
}
