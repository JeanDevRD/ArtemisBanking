using ArtemisBanking.Core.Application.ViewModels.Common;

namespace ArtemisBanking.Core.Application.Dtos.Loan
{
    public class ElegibleUserForLoanViewModel : CommonEntityViewModel<string>
    {
        public required string IdentificationNumber { get; set; }
        public required string FullName { get; set; }
        public required string Email { get; set; }
        public required decimal MonthlyIncome { get; set; }
    }
}
