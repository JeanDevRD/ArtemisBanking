namespace ArtemisBanking.Core.Application.ViewModel.Loan
{
    public class LoanDetailViewModel
    {
        public int LoanId { get; set; }
        public string? LoanNumber { get; set; }
        public string? ClientFullName { get; set; }
        public decimal Amount { get; set; }
        public decimal AnnualInterestRate { get; set; }
        public int TermMonths { get; set; }
        public DateTime ApprovedAt { get; set; }
        public List<InstallmentDetailViewModel>? Installments { get; set; }
    }
}
