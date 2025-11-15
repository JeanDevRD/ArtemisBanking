using ArtemisBanking.Core.Domain.Entities;
using ArtemisBanking.Core.Domain.Interfaces; // ✅ CORRECTO: usar la interfaz del dominio
using ArtemisBanking.Infraestructure.Persistence.Contexts;

namespace ArtemisBanking.Infraestructure.Persistence.Repositories
{
    public class BeneficiaryRepository : GenericRepository<Beneficiary>, IBeneficiaryRepository
    {
        public BeneficiaryRepository(ArtemisBankingContextSqlServer context)
            : base(context)
        {
        }
    }
}


