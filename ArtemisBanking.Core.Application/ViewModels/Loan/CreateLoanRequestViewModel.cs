namespace ArtemisBanking.Core.Application.ViewM.Loan
{
    public class CreateLoanRequestViewModel
    {
        public required string UserId { get; set; }
        public string? ApprovedByUserId { get; set; } 
        public decimal Amount { get; set; }
        public int TermMonths { get; set; } 
        public decimal AnnualInterestRate { get; set; }
    }
}
