using ArtemisBanking.Core.Application.Dtos.Common;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ArtemisBanking.Core.Application.Dtos.Beneficiary
{
    public class BeneficiaryDto : CommonEntityDto<int>
    {
        public required string FirstName { get; set; }
        public required string LastName { get; set; }
        public required string AccountNumber { get; set; }
        public required string UserId { get; set; }
    }
}
