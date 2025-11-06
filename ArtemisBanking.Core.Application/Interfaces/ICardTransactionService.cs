using ArtemisBanking.Core.Application.Dtos.CardTransaction;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ArtemisBanking.Core.Application.Interfaces
{
    public interface ICardTransactionService : IGenericService<CardTransactionDto>
    {
      Task<List<CardTransactionDto>> GetAllWithInclude();
    }
}
