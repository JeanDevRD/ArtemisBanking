namespace ArtemisBankingWebApp.Models
{
    public class ProductViewModel
    {
        public string Type { get; set; } = "";
        public string Number { get; set; } = "";
        public decimal Amount { get; set; }
        public string Status { get; set; } = "";
        public DateTime Date { get; set; }
        public int Id { get; set; }
        public int TypeId { get; set; } // 1=Account, 2=Loan, 3=Card
    }
}
