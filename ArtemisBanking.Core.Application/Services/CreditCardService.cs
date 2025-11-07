using ArtemisBanking.Core.Application.Dtos.CreditCard;
using ArtemisBanking.Core.Application.Interfaces;
using ArtemisBanking.Core.Domain.Entities;
using ArtemisBanking.Infraestructure.Persistence.Repositories;
using AutoMapper;


namespace ArtemisBanking.Core.Application.Services
{
    public class CreditCardService : GenericService<CreditCardDto, CreditCard>, ICreditCardService
    {
        private readonly ICreditCardRepository _creditCardRepository;
        private readonly IMapper _mapper;
        public CreditCardService(ICreditCardRepository genericRepository, IMapper mapper) : base(genericRepository, mapper)
        {
            _creditCardRepository = genericRepository;
            _mapper = mapper;
        }
        public override async Task<CreditCardDto> GetByIdAsync(int id)
        {
            try
            {
                var entities = await _creditCardRepository.GetAllListIncluideAsync(["CardTransactions"]);
               
                var entity = entities.FirstOrDefault(e => e.Id == id);
                if (entity == null)
                {
                    return null!;
                }
                return _mapper.Map<CreditCardDto>(entity);
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
                return null!;
            }
        }

        public async Task<List<CreditCardDto>> GetAllWithInclude()
        {
            try
            {
                var creditCards = await _creditCardRepository.GetAllListIncluideAsync(["CardTransactions"]);
                if (creditCards == null)
                {
                    return new List<CreditCardDto>();
                }
                return _mapper.Map<List<CreditCardDto>>(creditCards);
            }
            catch (Exception ex)
            {
                throw new Exception("Error retrieving credit cards with included data: " + ex.Message);
            }
        }

        
    }
}
