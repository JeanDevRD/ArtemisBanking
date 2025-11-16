using ArtemisBanking.Core.Application.ViewModels.Transaction;
using ArtemisBanking.Core.Application.ViewModels.Common;


namespace ArtemisBanking.Core.Application.ViewModel.SavingsAccount
{
    public class SavingsAccountViewModel : CommonEntityViewModel<int>
    {
        public required string AccountNumber { get; set; }
        public decimal Balance { get; set; }
        public required int Type { get; set; } 
        public bool IsActive { get; set; } = true;
        public DateTime CreatedAt { get; set; } = DateTime.Now;
        public required string UserId { get; set; }
        public string? AssignedByUserId { get; set; }
        public ICollection<TransactionViewModel>? Transactions { get; set; }
    }
}
