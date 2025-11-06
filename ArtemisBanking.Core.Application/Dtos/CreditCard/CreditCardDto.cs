using ArtemisBanking.Core.Application.Dtos.CardTransaction;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ArtemisBanking.Core.Application.Dtos.CreditCard
{
    public class CreditCardDto : CommonEntityDto<int>
    {
        public required string CardNumber { get; set; }
        public required string CvcHash { get; set; }
        public decimal CreditLimit { get; set; }
        public decimal CurrentDebt { get; set; }
        public DateTime ExpirationDate { get; set; }
        public bool IsActive { get; set; } = true;
        public required string UserId { get; set; }
        public required string AssignedByUserId { get; set; }
        public ICollection<CardTransactionDto>? CardTransactions { get; set; }
    }
}
