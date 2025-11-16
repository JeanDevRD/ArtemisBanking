using System.ComponentModel.DataAnnotations;

namespace ArtemisBanking.Core.Application.ViewModels.Transaction
{
    public class DepositTransactionViewModel
    {
        [Required(ErrorMessage = "El número de cuenta es obligatorio")]
        public string AccountNumber { get; set; } = string.Empty;

        [Required(ErrorMessage = "El monto es obligatorio")]
        [Range(0.01, double.MaxValue, ErrorMessage = "El monto debe ser mayor que cero")]
        public decimal Amount { get; set; }
    }
}
