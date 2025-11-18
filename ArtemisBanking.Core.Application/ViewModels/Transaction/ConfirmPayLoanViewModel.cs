using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ArtemisBanking.Core.Application.ViewModels.Transaction
{
    public class ConfirmPayLoanViewModel
    {
        public required string AccountNumber { get; set; }
        public decimal Amount { get; set; }
        public required string LoanNumber { get; set; }
        public required string LoanHolderName { get; set; }
        public decimal OutstandingAmount { get; set; }
    }
}

