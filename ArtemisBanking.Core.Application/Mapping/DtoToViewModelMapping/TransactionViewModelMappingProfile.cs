using ArtemisBanking.Core.Application.Dtos.SavingsAccount;
using ArtemisBanking.Core.Application.Dtos.Transaction;
using ArtemisBanking.Core.Application.ViewModel.SavingsAccount;
using ArtemisBanking.Core.Application.ViewModels.SavingsAccount;
using ArtemisBanking.Core.Application.ViewModels.Transaction;
using AutoMapper;

namespace ArtemisBanking.Core.Application.Mapping.DtoToViewModelMapping
{
    public class TransactionViewModelMappingProfile : Profile
    {
        public TransactionViewModelMappingProfile()
        {
            CreateMap<TransactionDto, TransactionViewModel>().ReverseMap();
            CreateMap<CountDepositDto, CountDepositViewModel>().ReverseMap();
            CreateMap<CountPaidsDto, CountPaidsViewModel>().ReverseMap();
            CreateMap<CountTransactionDto, CountTransactionViewModel>().ReverseMap();
            CreateMap<CountWithdrawalDto, CountWithdrawalViewModel>().ReverseMap();
            CreateMap<DepositTransactionDto, DepositTransactionViewModel>().ReverseMap();
            CreateMap<WithdrawalTransactionDto, WithdrawalTransactionViewModel>().ReverseMap();
            CreateMap<ProcessPaymentDto, ProcessPaymentViewModel>().ReverseMap();
            CreateMap<TransactionConfirmDto, TransactionConfirmViewModel>().ReverseMap();
        }
    }
}

