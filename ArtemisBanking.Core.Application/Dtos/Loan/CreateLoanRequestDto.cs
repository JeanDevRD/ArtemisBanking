namespace ArtemisBanking.Core.Application.Dtos.Loan
{
    public class CreateLoanRequestDto
    {
        public required string UserId { get; set; }
        public required string ApprovedByUserId { get; set; } 
        public decimal Amount { get; set; }
        public int TermMonths { get; set; } 
        public decimal AnnualInterestRate { get; set; }
    }
}
