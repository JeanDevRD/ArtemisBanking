
using ArtemisBanking.Core.Domain.Common;

namespace ArtemisBanking.Core.Domain.Entities
{
    public class Beneficiary : CommonEntity<int>
    {
        public required string FirstName { get; set; }
        public required string LastName { get; set; }
        public required string AccountNumber { get; set; }
        public required string UserId { get; set; }
    }
}
