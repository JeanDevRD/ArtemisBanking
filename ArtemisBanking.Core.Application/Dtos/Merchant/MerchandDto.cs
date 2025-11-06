using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ArtemisBanking.Core.Application.Dtos.Merchant
{
    public class MerchandDto : CommonEntityDto<int>
    {
        public required string Name { get; set; }
        public required string Email { get; set; }
        public bool IsActive { get; set; } = true;
        public DateTime CreatedAt { get; set; } = DateTime.Now;
        public required string AssociatedAccount { get; set; }
        public required string UserId { get; set; }
    }
}
