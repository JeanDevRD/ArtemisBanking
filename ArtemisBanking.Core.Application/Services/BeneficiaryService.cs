using ArtemisBanking.Core.Application.Dtos.Beneficiary;
using ArtemisBanking.Core.Application.Interfaces;
using ArtemisBanking.Core.Domain.Entities;
using ArtemisBanking.Infraestructure.Persistence.Repositories;
using AutoMapper;
namespace ArtemisBanking.Core.Application.Services
{
    public class BeneficiaryService : GenericService<BeneficiaryDto,Beneficiary>, IBeneficiaryService
    {
        public BeneficiaryService(IBeneficiaryRepository beneficiaryRepository, IMapper mapper) : base(beneficiaryRepository,mapper ) 
        { 
         
        }
    }
}
