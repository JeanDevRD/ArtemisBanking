namespace ArtemisBanking.Core.Application.ViewModels.SavingsAccount
{
    public class CreateSecundarySavingsAccountsViewModel
    {
        public string? UserId { get; set; }
        public decimal InitialBalance { get; set; }
        public string? AdminUserId { get; set; }
    }
}
