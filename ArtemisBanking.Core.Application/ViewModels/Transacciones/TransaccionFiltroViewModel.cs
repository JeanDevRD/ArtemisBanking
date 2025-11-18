using ArtemisBanking.Core.Application.Dtos.Transaction;

namespace ArtemisBanking.Core.Application.ViewModels.Transacciones
{
    public class TransaccionFiltroViewModel
    {
        public List<TransactionDto> Transacciones { get; set; } = new();
        public string? Tipo { get; set; } // Depósito, retiro, transferencia, etc.
        public string? Producto { get; set; } // Cuenta, préstamo, tarjeta
        public DateTime? FechaDesde { get; set; }
        public DateTime? FechaHasta { get; set; }
        public string? Buscar { get; set; } // Referencia, monto o descripción
    }
}

