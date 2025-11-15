using ArtemisBanking.Core.Domain.Common;

namespace ArtemisBanking.Core.Domain.Entities
{
    public class Transaction : CommonEntity<int>
    {
        // Registra todos los movimientos financieros de una cuenta de ahorro (depósitos, retiros,
        // transferencias, pagos) indicando origen, destino, tipo (DÉBITO/CRÉDITO) y estado.
        public DateTime Date { get; set; }
        public decimal Amount { get; set; }
        public required int Type { get; set; } // Si es una cuenta de DÉBITO o CRÉDITO
        public required int TypeTransaction { get; set; } // Si es depósito, retiro, transferencia, pago, etc.
        public required string Source { get; set; }
        public required string Beneficiary { get; set; }
        public required int Status { get; set; } // Si esta aprovada o rechazada 
        public int SavingsAccountId { get; set; }
        
        public required string CashierId { get; set; }

        public SavingsAccount SavingsAccount { get; set; } = null!;
    }
}
