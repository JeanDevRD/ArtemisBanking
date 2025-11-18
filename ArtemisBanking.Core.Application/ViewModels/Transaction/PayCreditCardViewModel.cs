using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ArtemisBanking.Core.Application.ViewModels.Transaction
{
    public class PayCreditCardViewModel
    {
        [Required(ErrorMessage = "El número de cuenta origen es obligatorio")]
        [StringLength(9, MinimumLength = 9, ErrorMessage = "Debe tener 9 dígitos")]
        [RegularExpression(@"^\d{9}$", ErrorMessage = "Solo números")]
        public string AccountNumber { get; set; } = string.Empty;

        [Required(ErrorMessage = "El monto es obligatorio")]
        [Range(0.01, double.MaxValue, ErrorMessage = "Debe ser mayor que cero")]
        public decimal Amount { get; set; }

        [Required(ErrorMessage = "El número de tarjeta es obligatorio")]
        [StringLength(16, MinimumLength = 16, ErrorMessage = "Debe tener 16 dígitos")]
        [RegularExpression(@"^\d{16}$", ErrorMessage = "Solo números")]
        public string CardNumber { get; set; } = string.Empty;
    }
}
