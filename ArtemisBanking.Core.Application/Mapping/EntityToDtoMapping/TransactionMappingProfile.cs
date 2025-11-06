using ArtemisBanking.Core.Application.Dtos.CardTransaction;
using ArtemisBanking.Core.Domain.Entities;
using AutoMapper;
using Microsoft.IdentityModel.Tokens;
namespace ArtemisBanking.Core.Application.Mapping.EntityToDtoMapping
{
    public class TransactionMappingProfile : Profile
    {
        public TransactionMappingProfile()
        {
            CreateMap<CardTransaction, CardTransactionDto>()
                .ReverseMap();
        }
    }
}
