using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ArtemisBanking.Core.Application.Dtos.Transaction
{
    public class ProcessPaymentDto
    {
        public required string CardNumber { get; set; }
        public required string MonthExpirationCard { get; set; }
        public required string YearExpirationCard { get; set; }
        public required string CVC { get; set; }
        public decimal TransactionAmount { get; set; }
    }
}
