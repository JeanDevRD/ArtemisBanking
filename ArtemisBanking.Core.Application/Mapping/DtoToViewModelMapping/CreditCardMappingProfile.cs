using ArtemisBanking.Core.Application.Dtos.CreditCard;
using ArtemisBanking.Core.Application.Dtos.Loan;
using ArtemisBanking.Core.Application.ViewModel.CreditCard;
using ArtemisBanking.Core.Application.ViewModel.Loan;
using AutoMapper;

namespace ArtemisBanking.Core.Application.Mapping.DtoToViewModelMapping
{
    public class CreditCardMappingProfile : Profile
    {
        public CreditCardMappingProfile()
        {
            CreateMap<ConsumptionDto, ConsumptionViewModel>().ReverseMap();

            CreateMap<CreditCardDetailDto, CreditCardDetailViewModel>().ReverseMap();

            CreateMap<CreditCardDto, CreditCardViewModel>().ReverseMap();

            CreateMap<CreditCardListDto, CreditCardListViewModel>().ReverseMap();

            CreateMap<CreditCardRequestDto, CreditCardRequestViewModel>().ReverseMap();

            CreateMap<ElegibleUserForCreditCardDto, ElegibleUserForCreditCardViewModel>().ReverseMap();

            CreateMap<UpdateCreditCardDto, UpdateCreditCardViewModel>().ReverseMap();
        }
    }
}

