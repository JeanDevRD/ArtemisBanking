using ArtemisBanking.Core.Application.ViewModels.Common;
using ArtemisBanking.Core.Application.ViewModels.Installment;


namespace ArtemisBanking.Core.Application.ViewModels.Loan
{
    public class LoanViewModel : CommonEntityViewModel<int>
    {
        public required string LoanNumber { get; set; }
        public decimal Amount { get; set; }
        public int TermMonths { get; set; }
        public decimal AnnualInterestRate { get; set; }
        public bool IsActive { get; set; } = true;
        public DateTime ApprovedAt { get; set; } = DateTime.Now;
        public required string ApprovedByUserId { get; set; }
        public required string UserId { get; set; }
        public ICollection<InstallmentViewModel>? Installments { get; set; }
    }
}
