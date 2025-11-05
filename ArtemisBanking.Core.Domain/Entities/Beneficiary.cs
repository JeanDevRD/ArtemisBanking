
using ArtemisBanking.Core.Domain.Common;

namespace ArtemisBanking.Core.Domain.Entities
{
    // Representa una cuenta de ahorro que el cliente agrega a su lista de contactos frecuentes
    // para facilitar transferencias sin necesidad de ingresar manualmente el número de cuenta cada vez.
    public class Beneficiary : CommonEntity<int>
    {
        public required string FirstName { get; set; }
        public required string LastName { get; set; }
        public required string AccountNumber { get; set; }
        public required string UserId { get; set; }
    }
}
