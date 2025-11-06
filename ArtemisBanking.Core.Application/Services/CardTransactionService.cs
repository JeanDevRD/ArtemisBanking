using ArtemisBanking.Core.Application.Dtos.CardTransaction;
using ArtemisBanking.Core.Application.Interfaces;
using ArtemisBanking.Core.Domain.Entities;
using ArtemisBanking.Infraestructure.Persistence.Repositories;
using AutoMapper;
using Microsoft.EntityFrameworkCore;

namespace ArtemisBanking.Core.Application.Services
{
    public class CardTransactionService : GenericService<CardTransactionDto, CardTransaction>, ICardTransactionService
    {
        private readonly ICardTransactionRepository _cardTransactionRepository;
        private readonly IMapper _mapper;
        public CardTransactionService(ICardTransactionRepository cardTransactionRepository, IMapper mapper) : base(cardTransactionRepository, mapper)
        {
            _cardTransactionRepository = cardTransactionRepository;
            _mapper = mapper;
        }

        public async Task<List<CardTransactionDto>> GetAllWithInclude()
        {
            try 
            {
                var cardTransactions = await _cardTransactionRepository.GetAllListIncluideAsync(["CreditCard"]);

                if (cardTransactions == null)
                {
                    return [];
                }

                return _mapper.Map<List<CardTransactionDto>>(cardTransactions);
            }
            catch (Exception ex)
            {
                throw new Exception("Error retrieving card transactions with included card: " + ex.Message);
            }

        }

    }
}
