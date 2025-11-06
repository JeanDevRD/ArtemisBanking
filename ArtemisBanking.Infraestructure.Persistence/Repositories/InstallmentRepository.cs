using ArtemisBanking.Core.Domain.Entities;
using ArtemisBanking.Infraestructure.Persistence.Contexts;

namespace ArtemisBanking.Infraestructure.Persistence.Repositories
{
    public class InstallmentRepository : GenericRepository<Installment>, IInstallmentRepository
    {
        public InstallmentRepository(ArtemisBankingContextSqlServer context) : base(context)
        {
        }
    }
}
