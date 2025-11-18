namespace ArtemisBanking.Core.Application.Dtos.Transaction
{
    public class TransactionConfirmDto
    {
        public int AccountId { get; set; }
        public required string AccountNumber { get; set; }
        public required string HolderName { get; set; }
        public decimal Amount { get; set; }
        public decimal CurrentDebt { get; set; }
        public decimal OutstandingAmount { get; set; }
    }
}
