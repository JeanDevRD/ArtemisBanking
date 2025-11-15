using ArtemisBanking.Core.Application.ViewModels.Common;
using ArtemisBanking.Core.Application.ViewModels.Loan;

namespace ArtemisBanking.Core.Application.ViewModels.Installment
{
    public class InstallmentViewModel : CommonEntityViewModel<int>
    {
        public DateTime PaymentDate { get; set; }
        public decimal PaymentAmount { get; set; }
        public bool IsPaid { get; set; } = false;
        public bool IsLate { get; set; } = false;
        public int LoanId { get; set; }
        
        // InstallmentDto.cs → AÑADIR
        public DateTime DueDate { get; set; }
        public LoanViewModel Loan { get; set; } = null!;
        public decimal Amount { get; set; }
    }
}
