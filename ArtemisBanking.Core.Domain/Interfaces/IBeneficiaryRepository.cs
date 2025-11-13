using ArtemisBanking.Core.Domain.Entities;
using ArtemisBanking.Infraestructure.Persistence.Repositories;

namespace ArtemisBanking.Core.Domain.Interfaces
{
    public interface IBeneficiaryRepository : IGenericRepository<Beneficiary>
    {
    }
}