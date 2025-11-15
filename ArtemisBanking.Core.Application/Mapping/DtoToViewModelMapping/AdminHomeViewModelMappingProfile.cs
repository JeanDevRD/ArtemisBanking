using ArtemisBanking.Core.Application.Dtos.AdminDashboard;
using ArtemisBanking.Core.Application.ViewModels.AdminDashboard;
using AutoMapper;

namespace ArtemisBanking.Core.Application.Mapping.DtoToViewModelMapping
{
    public class AdminHomeViewModelMappingProfile : Profile
    {
        public AdminHomeViewModelMappingProfile()
        {
            CreateMap<LoanAverageClientDto, LoanAverageClientViewModel>().ReverseMap();
            CreateMap<TotalClientsDto, TotalClientsViewModel>().ReverseMap();
            CreateMap<CreditCardCountDto, CreditCardCountViewModel>().ReverseMap();
            CreateMap<TotalFinancialProductDto, TotalFinancialProductViewModel>().ReverseMap();
            CreateMap<LoadCountDto, LoadCountViewModel>().ReverseMap();
            CreateMap<PaymentHistoryDto, PaymentHistoryViewModel>().ReverseMap();
            CreateMap<TotalSavingAccountDto, TotalSavingAccountViewModel>().ReverseMap();
            CreateMap<HistoricTransactionDto, HistoricTransactionViewModel>().ReverseMap();

        }
    }
}

