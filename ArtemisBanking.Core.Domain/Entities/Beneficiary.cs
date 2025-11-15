
using ArtemisBanking.Core.Domain.Common;

namespace ArtemisBanking.Core.Domain.Entities
{
    public class Beneficiary : CommonEntity<int>
    {
        public required string FirstName { get; set; }
        public required string LastName { get; set; }
        public required string AccountNumber { get; set; }
        public required string UserId { get; set; }

        // NUEVOS CAMPOS
        public required string Bank { get; set; }           // Ej: "Banco Popular", "Banreservas"
        public required int AccountType { get; set; }       // 1 = Ahorro, 2 = Corriente
    }
}
