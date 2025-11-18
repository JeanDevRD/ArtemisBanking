using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ArtemisBanking.Core.Application.ViewModels.Transaction
{
    public class ThirdPartyTransactionViewModel
    {
        [Required(ErrorMessage = "El número de cuenta origen es obligatorio")]
        public string AccountOrigin { get; set; } = string.Empty;

        [Required(ErrorMessage = "El número de cuenta destino es obligatorio")]
        public string AccountDestination { get; set; } = string.Empty;

        [Required(ErrorMessage = "El monto es obligatorio")]
        [Range(0.01, double.MaxValue, ErrorMessage = "El monto debe ser mayor que cero")]
        public decimal Amount { get; set; }
    }
}
