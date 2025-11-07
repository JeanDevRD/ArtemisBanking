using ArtemisBanking.Core.Application.Dtos.SavingsAccount;
using ArtemisBanking.Core.Application.Interfaces;
using ArtemisBanking.Core.Domain.Entities;
using ArtemisBanking.Infraestructure.Persistence.Repositories;
using AutoMapper;

namespace ArtemisBanking.Core.Application.Services
{
    public class SavingsAccountService : GenericService<SavingsAccountDto, SavingsAccount>, ISavingsAccountService
    {
        private readonly ISavingsAccountRepository _savingsAccountRepository;
        private readonly IMapper _mapper;
        public SavingsAccountService(ISavingsAccountRepository savingsAccountRepository, IMapper mapper) : base(savingsAccountRepository, mapper)
        {
            _savingsAccountRepository = savingsAccountRepository;
            _mapper = mapper;
        }
        public override async Task<SavingsAccountDto> GetByIdAsync(int id)
        {
            try
            {
                var entities = await _savingsAccountRepository.GetAllListIncluideAsync(["Transactions"]);

                var entity = entities.FirstOrDefault(e => e.Id == id);
                if (entity == null)
                {
                    return null!;
                }
                return _mapper.Map<SavingsAccountDto>(entity);
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
                return null!;
            }
        }

        public async Task<List<SavingsAccountDto>> GetAllWithInclude()
        {
            try
            {
                var creditCards = await _savingsAccountRepository.GetAllListIncluideAsync(["Transactions"]);
                if (creditCards == null)
                {
                    return new List<SavingsAccountDto>();
                }
                return _mapper.Map<List<SavingsAccountDto>>(creditCards);
            }
            catch (Exception ex)
            {
                throw new Exception("Error retrieving credit cards with included data: " + ex.Message);
            }
        }
    }
}
