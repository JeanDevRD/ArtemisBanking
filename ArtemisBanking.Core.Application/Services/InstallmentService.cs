using ArtemisBanking.Core.Application.Dtos.CreditCard;
using ArtemisBanking.Core.Application.Dtos.Installment;
using ArtemisBanking.Core.Application.Interfaces;
using ArtemisBanking.Core.Domain.Entities;
using ArtemisBanking.Infraestructure.Persistence.Repositories;
using AutoMapper;

namespace ArtemisBanking.Core.Application.Services
{
   
    public class InstallmentService : GenericService<InstallmentDto,Installment>, IInstallmentService
    {
        private readonly IInstallmentRepository _InstallmentRepository;
        private readonly IMapper _mapper;
        public InstallmentService(IMapper mapper, IInstallmentRepository installmentRepository) : base(installmentRepository,mapper)
        { 
          _InstallmentRepository = installmentRepository;
          _mapper = mapper;
        }
        public async Task<List<InstallmentDto>> GetAllWithInclude()
        {
            try
            {
                var creditCards = await _InstallmentRepository.GetAllListIncluideAsync(["Loan"]);
                if (creditCards == null)
                {
                    return new List<InstallmentDto>();
                }
                return _mapper.Map<List<InstallmentDto>>(creditCards);
            }
            catch (Exception ex)
            {
                throw new Exception("Error retrieving credit cards with included data: " + ex.Message);
            }
        }

    }
}
