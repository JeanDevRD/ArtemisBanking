using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ArtemisBanking.Core.Application.Dtos.SavingsAccount
{
    public class SavingsAccountsHomeDto
    {
        public int? AccountId { get; set; }
        public string? AccountNumber { get; set; }
        public string? UserFullName { get; set; }
        public decimal Balance { get; set; }
        public string? AccountType { get; set; }
        public bool IsActive { get; set; }
        public bool IsPrincipal { get; set; }
    }
}
