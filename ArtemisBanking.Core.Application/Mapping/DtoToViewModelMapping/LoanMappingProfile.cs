using ArtemisBanking.Core.Application.Dtos.Loan;
using ArtemisBanking.Core.Application.ViewM.Loan;
using ArtemisBanking.Core.Application.ViewModel.Loan;
using ArtemisBanking.Core.Application.ViewModels.Loan;
using AutoMapper;

namespace ArtemisBanking.Core.Application.Mapping.DtoToViewModelMapping
{
    public class LoanMappingProfile : Profile
    {
        public LoanMappingProfile()
        {
            CreateMap<CreateLoanRequestDto, CreateLoanRequestViewModel>().ReverseMap();
            CreateMap<ElegibleUserForLoanDto, ElegibleUserForLoanViewModel>().ReverseMap();
            CreateMap<InstallmentDetailDto, InstallmentDetailViewModel>().ReverseMap();
            CreateMap<LoanDetailDto, LoanDetailViewModel>().ReverseMap();
            CreateMap<LoanListDto, LoanListViewModel>().ReverseMap();
            CreateMap<LoanDto, LoanViewModel>().ReverseMap();
            CreateMap<UpdateInterestRateDto, UpdateInterestRateViewModel>().ReverseMap();
        }
    }
}

