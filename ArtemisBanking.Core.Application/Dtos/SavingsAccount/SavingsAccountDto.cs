using ArtemisBanking.Core.Application.Dtos.Common;
using ArtemisBanking.Core.Application.Dtos.Transaction;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ArtemisBanking.Core.Application.Dtos.SavingsAccount
{
    public class SavingsAccountDto : CommonEntityDto<int>
    {
        public required string AccountNumber { get; set; }
        public decimal Balance { get; set; }
        public required int Type { get; set; } 
        public bool IsActive { get; set; } = true;
        public DateTime CreatedAt { get; set; } = DateTime.Now;
        public required string UserId { get; set; }
        public string? AssignedByUserId { get; set; }
        public ICollection<TransactionDto>? Transactions { get; set; }
    }
}
