using ArtemisBanking.Core.Application.Dtos.CreditCard;
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
        public async Task<List<LoanDto>> GetAllWithInclude()
        {
            try
            {
                var creditCards = await _loanRepository.GetAllListIncluideAsync(["Installments"]);
                if (creditCards == null)
                {
                    return new List<LoanDto>();
                }
                return _mapper.Map<List<LoanDto>>(creditCards);
            }
            catch (Exception ex)
            {
                throw new Exception("Error retrieving credit cards with included data: " + ex.Message);
            }
        }
        public override async Task<LoanDto> GetByIdAsync(int id)
        {
            try
            {
                var entities = await _loanRepository.GetAllListIncluideAsync(["Installments"]);

                var entity = entities.FirstOrDefault(e => e.Id == id);
                if (entity == null)
                {
                    return null!;
                }
                return _mapper.Map<LoanDto>(entity);
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
                return null!;
            }
        }
    }
}
