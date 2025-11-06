using ArtemisBanking.Core.Application.Dtos.Transaction;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ArtemisBanking.Core.Application.Interfaces
{
    public interface ITransactionService : IGenericService<TransactionDto>
    {
    }
}
