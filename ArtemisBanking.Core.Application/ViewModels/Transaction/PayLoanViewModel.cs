using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ArtemisBanking.Core.Application.ViewModels.Transaction
{
    public class PayLoanViewModel
    {
        [Required(ErrorMessage = "El número de cuenta origen es obligatorio")]
        public string AccountNumber { get; set; } = string.Empty;

        [Required(ErrorMessage = "El número de préstamo es obligatorio")]
        [StringLength(9, MinimumLength = 9, ErrorMessage = "El número de préstamo debe tener 9 dígitos")]
        public string LoanNumber { get; set; } = string.Empty;

        [Required(ErrorMessage = "El monto es obligatorio")]
        [Range(0.01, double.MaxValue, ErrorMessage = "El monto debe ser mayor que cero")]
        public decimal Amount { get; set; }
    }
}
