using ArtemisBanking.Core.Application.Dtos.SavingsAccount;
using ArtemisBanking.Core.Application.Dtos.Transaction;
using ArtemisBanking.Core.Application.Dtos.User;
using ArtemisBanking.Core.Application.ViewModel.SavingsAccount;
using ArtemisBanking.Core.Application.ViewModels.SavingsAccount;

using AutoMapper;

namespace ArtemisBanking.Core.Application.Mapping.DtoToViewModelMapping
{
    public class SavingAccountViewModelMappingProfile : Profile
    {
        public SavingAccountViewModelMappingProfile()
        {
            CreateMap<CreateSecundarySavingsAccountsDto, CreateSecundarySavingsAccountsViewModel>().ReverseMap();
            CreateMap<SavingsAccountsHomeDto, SavingsAccountsHomeViewModel>().ReverseMap();
            CreateMap<SavingsAccountDetailDto, SavingsAccountDetailViewModel>().ReverseMap();
            CreateMap<TransactionDetailDto, TransactionDetailViewModel>().ReverseMap();
            CreateMap<SavingsAccountDto, SavingsAccountViewModel>().ReverseMap();
        }
    }
}

