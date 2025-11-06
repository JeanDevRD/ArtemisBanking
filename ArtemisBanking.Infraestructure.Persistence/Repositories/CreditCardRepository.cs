using ArtemisBanking.Core.Domain.Entities;
using ArtemisBanking.Infraestructure.Persistence.Contexts;

namespace ArtemisBanking.Infraestructure.Persistence.Repositories
{
    public class CreditCardRepository : GenericRepository<CreditCard>, ICreditCardRepository
    {
        public CreditCardRepository(ArtemisBankingContextSqlServer context) : base(context)
        {
        }
    }
}
