namespace ArtemisBanking.Core.Application.ViewModel.CreditCard
{
    public class CreditCardRequestViewModel
    {
        public required string IdCliente { get; set; }
        public required decimal CreditLimit { get; set; }
    }
}
