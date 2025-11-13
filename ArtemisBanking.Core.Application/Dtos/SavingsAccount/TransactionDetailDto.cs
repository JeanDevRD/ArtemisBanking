using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ArtemisBanking.Core.Application.Dtos.SavingsAccount
{
    public class TransactionDetailDto
    {
        public int TransactionId { get; set; }
        public DateTime TransactionDate { get; set; }
        public decimal Amount { get; set; }
        public string? TransactionType { get; set; }
        public string? Beneficiary { get; set; }
        public string? Origin { get; set; }
        public string? Status { get; set; } 
    }
}
