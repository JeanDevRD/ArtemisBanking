namespace ArtemisBanking.Core.Application.ViewModels.SavingsAccount
{
    public class SavingsAccountsHomeViewModel
    {
        public int? AccountId { get; set; }
        public string? AccountNumber { get; set; }
        public string? UserFullName { get; set; }
        public decimal Balance { get; set; }
        public string? AccountType { get; set; }
        public bool IsActive { get; set; }
        public bool IsPrincipal { get; set; }
    }
}
