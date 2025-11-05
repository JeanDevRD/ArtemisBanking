using ArtemisBanking.Core.Domain.Common;

namespace ArtemisBanking.Core.Domain.Entities
{
    public class Merchant : CommonEntity<int>
    {
        //Representa un comercio o merchant registrado en el sistema que puede recibir pagos
        // a través del procesador Hermes Pay utilizando tarjetas de crédito.
        public required string Name { get; set; }
        public required string Email { get; set; }
        public bool IsActive { get; set; } = true; 
        public DateTime CreatedAt { get; set; } = DateTime.Now;
        public required string AssociatedAccount { get; set; }
        public required string UserId { get; set; }
    }
}
