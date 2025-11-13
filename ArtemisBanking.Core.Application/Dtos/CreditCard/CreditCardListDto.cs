namespace ArtemisBanking.Core.Application.Dtos.CreditCard
{
    public class CreditCardListDto
    {
        public required string CardNumber { get; set; }
        public required string FullNameUser { get; set; }
        public decimal CreditLimit { get; set; }
        public DateTime ExpirationDate { get; set; }
        public decimal CurrentDebt { get; set; }
    }
}
