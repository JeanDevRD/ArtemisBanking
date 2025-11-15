namespace ArtemisBanking.Core.Application.ViewModel.Loan
{
    public class InstallmentDetailViewModel
    {
        public int InstallmentId { get; set; }
        public DateTime PaymentDate { get; set; }
        public decimal PaymentAmount { get; set; }
        public bool IsPaid { get; set; }
        public bool IsLate { get; set; }
    }
}
