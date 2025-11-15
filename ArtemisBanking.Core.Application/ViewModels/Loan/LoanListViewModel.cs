namespace ArtemisBanking.Core.Application.ViewModel.Loan
{
    public class LoanListViewModel
    {
        public int LoanId { get; set; } 
        public required string ClientFullName { get; set; } 
        public decimal CapitalAmount { get; set; }
        public int TotalInstallments { get; set; }
        public int PaidInstallments { get; set; } 
        public decimal OutstandingAmount { get; set; } 
        public decimal InterestRate { get; set; } 
        public int TermInMonths { get; set; } 
        public required string PaymentStatus { get; set; } 
    }
}
