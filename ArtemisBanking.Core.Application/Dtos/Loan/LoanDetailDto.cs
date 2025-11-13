using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ArtemisBanking.Core.Application.Dtos.Loan
{
    public class LoanDetailDto
    {
        public int LoanId { get; set; }
        public string? LoanNumber { get; set; }
        public string? ClientFullName { get; set; }
        public decimal Amount { get; set; }
        public decimal AnnualInterestRate { get; set; }
        public int TermMonths { get; set; }
        public DateTime ApprovedAt { get; set; }
        public List<InstallmentDetailDto>? Installments { get; set; }
    }
}
