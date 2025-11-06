using ArtemisBanking.Core.Application.Dtos.CreditCard;
using ArtemisBanking.Core.Application.Interfaces;
using ArtemisBanking.Core.Domain.Entities;
using ArtemisBanking.Infraestructure.Persistence.Repositories;
using AutoMapper;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

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
