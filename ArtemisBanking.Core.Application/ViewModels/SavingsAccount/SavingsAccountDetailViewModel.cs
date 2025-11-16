using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ArtemisBanking.Core.Application.ViewModels.SavingsAccount
{
    public class SavingsAccountDetailViewModel
    {
        public int AccountId { get; set; }
        public string? AccountNumber { get; set; }
        public string? ClientFullName { get; set; }
        public decimal Balance { get; set; }
        public string? AccountType { get; set; }
        public List<TransactionDetailViewModel>? Transactions { get; set; }
    }
}
