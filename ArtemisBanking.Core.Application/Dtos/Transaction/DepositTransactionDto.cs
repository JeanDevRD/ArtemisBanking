namespace ArtemisBanking.Core.Application.Dtos.Transaction
{
    public class DepositTransactionDto
    {
        public required string AccountNumber { get; set; }
        public required decimal Amount { get; set; }
    }
}
