namespace ArtemisBanking.Core.Application.ViewModels.SavingsAccount
{
    public class HomeSavingsAccountViewModel
    {
        public List<SavingsAccountsHomeViewModel> Accounts { get; set; } = new();

        public int PageNumber { get; set; }
        public int TotalPages { get; set; }

        public string? FilterIdentificationNumber { get; set; }
        public bool? IsActive { get; set; }
        public int? AccountType { get; set; }
    }
}
