using ArtemisBanking.Core.Application.Dtos.Loan;
using ArtemisBanking.Core.Application.Interfaces;
using ArtemisBanking.Core.Domain.Entities;
using ArtemisBanking.Infraestructure.Persistence.Repositories;
using AutoMapper;

namespace ArtemisBanking.Core.Application.Services
{
    public class LoanService : GenericService<LoanDto,Loan>, ILoanService
    {
        private readonly ILoanRepository _loanRepository;
        private readonly IMapper _mapper;
        public LoanService(ILoanRepository loanRepository,IMapper mapper) : base(loanRepository, mapper) 
        { 
         _loanRepository = loanRepository;
         _mapper = mapper;
        }
    }
}
