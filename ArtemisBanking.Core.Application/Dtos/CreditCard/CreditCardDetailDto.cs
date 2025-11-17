using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ArtemisBanking.Core.Application.Dtos.CreditCard
{
    public class CreditCardDetailDto
    {
        public required string cardId { get; set; }
        public required string cardNumberMasked { get; set; }
        public List<ConsumptionDto>? consumptions { get; set; } = new List<ConsumptionDto>();
    }
}
