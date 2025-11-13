using ArtemisBanking.Core.Application.Dtos.Common;

namespace ArtemisBanking.Core.Application.Dtos.Loan
{
    public class ElegibleUserForCreditCardDto : CommonEntityDto<string>
    {
        public required string IdentificationNumber { get; set; }
        public required string FullName { get; set; }
        public required string Email { get; set; }
        public required decimal MonthlyIncome { get; set; }
    }
}
