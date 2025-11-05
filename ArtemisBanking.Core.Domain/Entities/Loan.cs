using ArtemisBanking.Core.Domain.Common;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ArtemisBanking.Core.Domain.Entities
{
    // Un préstamo bancario otorgado a un cliente con su sistema de amortización,
    // tasa de interés, y plazo definido en meses.
    public class Loan : CommonEntity<int>
    {
        public required string LoanNumber { get; set; }
        public decimal Amount { get; set; }
        public int TermMonths { get; set; }
        public decimal AnnualInterestRate { get; set; }
        public bool IsActive { get; set; } = true;
        public DateTime ApprovedAt { get; set; } = DateTime.Now;
        public required string ApprovedByUserId { get; set; }
        public required string UserId { get; set; }
        public ICollection<Installment>? Installments { get; set; }
    }
}
