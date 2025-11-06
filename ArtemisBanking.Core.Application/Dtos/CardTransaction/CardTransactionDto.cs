using ArtemisBanking.Core.Application.Dtos.CreditCard;
using ArtemisBanking.Core.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ArtemisBanking.Core.Application.Dtos.CardTransaction
{
    public class CardTransactionDto : CommonEntityDto<int>
    {
        public DateTime Date { get; set; }
        public decimal Amount { get; set; }
        public required string Merchant { get; set; }
        public required int Status { get; set; }
        public int CreditCardId { get; set; }
        public CreditCardDto CreditCard { get; set; } = null!;
    }
}
