using ArtemisBanking.Core.Application.Dtos.Common;
using ArtemisBanking.Core.Application.Dtos.Loan;

namespace ArtemisBanking.Core.Application.Dtos.Installment
{
    public class InstallmentDto : CommonEntityDto<int>
    {
        public DateTime PaymentDate { get; set; }
        public decimal PaymentAmount { get; set; }
        public bool IsPaid { get; set; } = false;
        public bool IsLate { get; set; } = false;
        public int LoanId { get; set; }
        
        // InstallmentDto.cs → AÑADIR
        public DateTime DueDate { get; set; }
        public LoanDto Loan { get; set; } = null!;
        public decimal Amount { get; set; }
    }
}
