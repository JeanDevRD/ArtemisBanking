using ArtemisBanking.Core.Application.Dtos.AdminDashboard;
using ArtemisBanking.Core.Application.Dtos.Common;
using ArtemisBanking.Core.Application.Dtos.CreditCard;
using ArtemisBanking.Core.Application.Dtos.Loan;
using ArtemisBanking.Core.Application.Interfaces;
using ArtemisBanking.Core.Domain.Entities;
using ArtemisBanking.Infraestructure.Persistence.Repositories;
using AutoMapper;
using Microsoft.EntityFrameworkCore;

namespace ArtemisBanking.Core.Application.Services
{
    public class LoanService : GenericService<LoanDto,Loan>, ILoanService
    {
        private readonly ILoanRepository _loanRepository;
        private readonly IAccountServiceForApp _clientApp;
        private readonly IMapper _mapper;
        public LoanService(ILoanRepository loanRepository,IMapper mapper, IAccountServiceForApp clientApp) : base(loanRepository, mapper) 
        { 
            _loanRepository = loanRepository;
            _mapper = mapper;
            _clientApp = clientApp;

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

        public async Task<ResultDto<LoanDto>> AddLoanAsync(LoanDto entityDto)
        {
            var result = new ResultDto<LoanDto>();

            var loan = await _loanRepository.GetAllQueryAsync()
                .Where(x => x.UserId == entityDto.UserId && x.IsActive)
                .FirstOrDefaultAsync();

            if (loan != null)
            {
                result.IsError = true;
                result.Message = "Este cliente ya tiene un prestamo activo";
                return result;
            }

            var newLoan = await base.AddAsync(entityDto);

            result.IsError = false;
            result.Result = newLoan;
            return result;
        }




        public async Task<ResultDto<List<LoanListDto>>?> GetAllActiveLoanAsync()
        {
            var result = new ResultDto<List<LoanListDto>>();
            try
            {
                var loanActive = await _loanRepository.GetAllQueryAsync().Where(x => x.IsActive).OrderByDescending(x => x.ApprovedAt).ToListAsync();

                if (!loanActive.Any())
                {
                    result.IsError = true;
                    result.Message = "No hay préstamos activos";
                    return result;
                }

                var loans = new List<LoanListDto>();

                foreach (var loan in loanActive)
                {
                    var client = await _clientApp.GetUserById(loan.UserId);

                    if (client == null)
                    {
                        result.IsError = true;
                        result.Message = $"No se encontraron los datos de este cliente";
                        return result;
                    }

                    loans.Add(new LoanListDto
                    {
                        LoanId = loan.Id,
                        ClientFullName = $"{client.FirstName} {client.LastName}",
                        CapitalAmount = loan.Amount,
                        TotalInstallments = loan.Installments?.Count ?? 0,
                        PaidInstallments = loan.Installments?.Count(i => i.IsPaid) ?? 0,
                        OutstandingAmount = loan.Installments?.Where(i => !i.IsPaid).Sum(i => i.PaymentAmount) ?? 0,
                        InterestRate = loan.AnnualInterestRate,
                        TermInMonths = loan.TermMonths,
                        PaymentStatus = loan.Installments != null && loan.Installments.Any(i => i.IsLate)? "En mora" : "Al dia"
                    });
                }

                result.IsError = false;
                result.Result = loans;
            }
            catch (Exception ex)
            {
                result.IsError = true;
                result.Message = ex.Message;
            }

            return result;
        }


        public async Task<ResultDto<List<LoanListDto>>> GetLoansByUserIdentity(string identificationNumber, bool isActive)
        {
            var result = new ResultDto<List<LoanListDto>>();

            try
            {
                var client = await _clientApp.GetUserByIdentificationNumber(identificationNumber);

                if (client == null)
                {
                    result.IsError = true;
                    result.Message = "Usuario no encontrado";
                    return result;
                }

                var loansList = await _loanRepository.GetAllQueryAsync().Where(x => x.UserId == client.Id && x.IsActive == isActive).OrderByDescending(x => x.ApprovedAt).ToListAsync();

                if (loansList == null)
                {
                    result.IsError = true;
                    result.Message = "No se encontraron prestamos para este usuario";
                    return result;
                }

                var loansDto = loansList.Select(loan => new LoanListDto
                {
                    LoanId = loan.Id,
                    ClientFullName = $"{client.FirstName} {client.LastName}",
                    CapitalAmount = loan.Amount,
                    TotalInstallments = loan.Installments?.Count ?? 0,
                    PaidInstallments = loan.Installments?.Count(i => i.IsPaid) ?? 0,
                    OutstandingAmount = loan.Installments?.Where(i => !i.IsPaid).Sum(i => i.PaymentAmount) ?? 0,
                    InterestRate = loan.AnnualInterestRate,
                    TermInMonths = loan.TermMonths,
                    PaymentStatus = loan.Installments != null && loan.Installments.Any(i => i.IsLate) ? "En mora" : "Al día"
                }).ToList();

                result.IsError = false;
                result.Result = loansDto;
            }
            catch (Exception ex)
            {
                result.IsError = true;
                result.Message = ex.Message;
            }

            return result;
        }
    }
}
