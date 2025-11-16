using ArtemisBanking.Core.Application.ViewModel.Loan;

namespace ArtemisBanking.Core.Application.ViewModels.Loan
{
    public class HomeLoanViewModel
    {
        public List<LoanListViewModel> Loans { get; set; } = new();
        public int PageNumber { get; set; }
        public int TotalPages { get; set; }
    }
}
