namespace ArtemisBanking.Core.Application.ViewModels.SavingsAccount
{
    public class TransactionDetailViewModel
    {
        public int TransactionId { get; set; }
        public DateTime TransactionDate { get; set; }
        public decimal Amount { get; set; }
        public string? TransactionType { get; set; }
        public string? Beneficiary { get; set; }
        public string? Origin { get; set; }
        public string? Status { get; set; } 
    }
}
