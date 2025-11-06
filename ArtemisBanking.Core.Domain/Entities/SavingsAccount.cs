using ArtemisBanking.Core.Domain.Common;
using System.Transactions;

namespace ArtemisBanking.Core.Domain.Entities
{
    public class SavingsAccount : CommonEntity<int>
    {
        // Una cuenta bancaria de ahorro que puede ser principal o secundaria,
        // donde se realizan depósitos, retiros, y se reciben desembolsos de préstamos.
        public required string AccountNumber { get; set; }
        public decimal Balance { get; set; }
        public required int Type { get; set; } // Ej : Principal o Secundaria (ojo: en la BD va en inglés)
        public bool IsActive { get; set; } = true;
        public DateTime CreatedAt { get; set; } = DateTime.Now;
        public required string UserId { get; set; }
        public string? AssignedByUserId { get; set; }
        public ICollection<Transaction>? Transactions { get; set; }
    }
}
