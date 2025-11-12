using ArtemisBanking.Core.Domain.Entities;
using ArtemisBanking.Infraestructure.Persistence.Contexts;

namespace ArtemisBanking.Infraestructure.Persistence.Repositories
{
    public class SavingsAccountRepository : GenericRepository<SavingsAccount>, ISavingsAccountRepository
    {
        public SavingsAccountRepository(ArtemisBankingContextSqlServer context) : base(context)
        {
        }


    }
}
