using ArtemisBanking.Core.Application.ViewModels.Common;

namespace ArtemisBanking.Core.Application.ViewModel.Loan
{
    public class ElegibleUserForCreditCardViewModel : CommonEntityViewModel<string>
    {
        public required string IdentificationNumber { get; set; }
        public required string FullName { get; set; }
        public required string Email { get; set; }
        public required decimal MonthlyIncome { get; set; }
    }
}
