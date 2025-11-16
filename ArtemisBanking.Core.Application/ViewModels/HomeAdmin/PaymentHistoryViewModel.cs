namespace ArtemisBanking.Core.Application.ViewModels.AdminDashboard
{
    public class PaymentHistoryViewModel
    {
        public required int TotalPaymentHistory { get; set; } 
        public required int PaymentHistoryForDay { get; set; }
    }
}
