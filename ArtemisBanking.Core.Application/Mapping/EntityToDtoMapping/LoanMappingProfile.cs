using ArtemisBanking.Core.Application.Dtos.Loan;
using ArtemisBanking.Core.Domain.Entities;
using AutoMapper;

namespace ArtemisBanking.Core.Application.Mapping.EntityToDtoMapping
{
    public class LoanMappingProfile : Profile
    {
        public LoanMappingProfile()
        {
            CreateMap<Loan, LoanDto>()
                .ReverseMap();

           CreateMap<Loan, LoanDetailDto>()
          .ForMember(dest => dest.LoanId, opt => opt.MapFrom(src => src.Id))
          .ForMember(dest => dest.ClientFullName, opt => opt.Ignore()) 
          .ForMember(dest => dest.Installments, opt => opt.MapFrom(src =>
           src.Installments!.OrderBy(i => i.PaymentDate).ToList()));

            CreateMap<Loan, LoanListDto>()
           .ForMember(dest => dest.LoanId, opt => opt.MapFrom(src => src.Id))
           .ForMember(dest => dest.ClientFullName, opt => opt.Ignore()) 
           .ForMember(dest => dest.CapitalAmount, opt => opt.MapFrom(src => src.Amount))
           .ForMember(dest => dest.TotalInstallments, opt => opt.MapFrom(src => src.Installments != null ? src.Installments.Count : 0))
           .ForMember(dest => dest.PaidInstallments, opt => opt.MapFrom(src => src.Installments != null ? src.Installments.Count(i => i.IsPaid) : 0))
           .ForMember(dest => dest.OutstandingAmount, opt => opt.MapFrom(src => src.Installments != null ? src.Installments.Where(i => !i.IsPaid).Sum(i => i.PaymentAmount) : 0))
           .ForMember(dest => dest.InterestRate, opt => opt.MapFrom(src => src.AnnualInterestRate))
           .ForMember(dest => dest.TermInMonths, opt => opt.MapFrom(src => src.TermMonths))
           .ForMember(dest => dest.PaymentStatus, opt => opt.MapFrom(src =>
               src.Installments != null && src.Installments.Any(i => i.IsLate) ? "En mora" : "Al día"));
        }
    }
}
