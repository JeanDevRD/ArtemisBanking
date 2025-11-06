using ArtemisBanking.Core.Application.Dtos.CardTransaction;
using ArtemisBanking.Core.Domain.Entities;
using AutoMapper;
namespace ArtemisBanking.Core.Application.Mapping.EntityToDtoMapping
{
    public class CardTransacctionMappingProfile : Profile
    {
        public CardTransacctionMappingProfile()
        {
            CreateMap<CardTransaction, CardTransactionDto>()
                .ReverseMap();
        }
    }
}
