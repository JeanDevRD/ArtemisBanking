using ArtemisBanking.Core.Application.ViewModels.AdminDashboard;

namespace ArtemisBanking.Core.Application.ViewModels.HomeAdmin
{
    public class HomeAdminViewModel
    {
        public CreditCardCountViewModel? creditCardCount { get; set; }
        public HistoricTransactionViewModel? historicTransaction { get; set; }
        public LoadCountViewModel? loadCount { get; set; }
        public LoanAverageClientViewModel? loanAverageClient { get; set; }
        public PaymentHistoryViewModel? paymentHistory { get; set; }
        public TotalClientsViewModel? clientsViewModel { get; set; }
        public TotalFinancialProductViewModel? totalFinancial { get; set; }
        public TotalSavingAccountViewModel? totalSaving { get; set; }
    }
}
