using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ArtemisBanking.Core.Application.Dtos.Merchant
{
    public class UpdateMerchantDto
    {
        public required string Name { get; set; }
        public required string Email { get; set; }
    }
}
