namespace ArtemisBanking.Core.Application.Dtos.CreditCard
{
    public class CreditCardRequestDto
    {
        public required string IdCliente { get; set; }
        public required decimal CreditLimit { get; set; }
    }
}
