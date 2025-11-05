using ArtemisBanking.Core.Domain.Common;

namespace ArtemisBanking.Core.Domain.Entities
{
 //Cuota individual dentro de la tabla de amortización de un préstamo,
 // con su fecha de vencimiento, monto a pagar, y control de si fue pagada o está atrasada.
    public class Installment : CommonEntity<int>
    {
        public DateTime PaymentDate { get; set; }
        public decimal PaymentAmount { get; set; }
        public bool IsPaid { get; set; } = false;
        public bool IsLate { get; set; } = false;
        public int LoanId { get; set; }
        public Loan Loan { get; set; } = null!;
    }
}
