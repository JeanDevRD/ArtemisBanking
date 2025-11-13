using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ArtemisBanking.Core.Application.Dtos.CreditCard
{
    public class CreditCardDetailDto
    {
        public required string cardId;                
        public required string cardNumberMasked;       
        public List<ConsumptionDto>? consumptions;
    }
}
