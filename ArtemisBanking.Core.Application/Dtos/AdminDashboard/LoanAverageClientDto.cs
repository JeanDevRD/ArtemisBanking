using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ArtemisBanking.Core.Application.Dtos.AdminDashboard
{
    public class LoanAverageClientDto
    {
        public double AverageLoanAmount { get; set; }
        public int TotalClients { get; set; }
    }
}
