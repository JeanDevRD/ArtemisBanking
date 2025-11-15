using ArtemisBanking.Core.Application.Dtos.Common;
using ArtemisBanking.Core.Domain.Enum;

namespace ArtemisBanking.Core.Application.Dtos.Beneficiary
{
    public class BeneficiaryDto : CommonEntityDto<int>
    {
        public required string FirstName { get; set; }
        public required string LastName { get; set; }
        public required string AccountNumber { get; set; }
        public required string UserId { get; set; }

        // NUEVOS CAMPOS
        public required string Bank { get; set; }
        public required int AccountType { get; set; }
        public string AccountTypeName => ((AccountType)AccountType).ToString();
        public required string Cedula { get; set; }
    }
}
