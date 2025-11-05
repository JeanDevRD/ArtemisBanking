using ArtemisBanking.Core.Domain.Common;

namespace ArtemisBanking.Core.Domain.Entities
{
    public class Marchant : CommonEntity<int>
    {
        public required string Name { get; set; }
        public required string Email { get; set; }
        public required string AssociatedAccount { get; set; }
        public required string UserId { get; set; }
    }
}
