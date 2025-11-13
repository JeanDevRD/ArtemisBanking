 using ArtemisBanking.Core.Application.Dtos.Common;
using ArtemisBanking.Core.Application.Dtos.Email;
using ArtemisBanking.Core.Application.Dtos.Installment;
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
        private readonly ISavingsAccountService _savingsAccountService;
        private readonly IEmailService _emailService;
        private readonly IMapper _mapper;
        public LoanService(ILoanRepository loanRepository,IMapper mapper, IAccountServiceForApp clientApp, ISavingsAccountService savingsAccountService, IEmailService emailService) : base(loanRepository, mapper) 
        { 
            _loanRepository = loanRepository;
            _mapper = mapper;
            _clientApp = clientApp;
            _savingsAccountService = savingsAccountService;
            _emailService = emailService;

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

        public async Task<ResultDto<LoanDto>> AddLoanAsync(CreateLoanRequestDto dto)
        {
            var result = new ResultDto<LoanDto>();
            try
            {
                var client = await _clientApp.GetUserById(dto.UserId);
                if (client == null || !client.IsActive)
                {
                    result.IsError = true;
                    result.Message = "Cliente no válido o inactivo";
                    return result;
                }

                var hasActiveLoan = await _loanRepository.GetAllQueryAsync()
                    .AnyAsync(l => l.UserId == client.Id && l.IsActive);
                if (hasActiveLoan)
                {
                    result.IsError = true;
                    result.Message = "El cliente ya tiene un préstamo activo";
                    return result;
                }

                var validTerms = new[] { 6, 12, 18, 24, 30, 36, 42, 48, 54, 60 };
                if (!validTerms.Contains(dto.TermMonths))
                {
                    result.IsError = true;
                    result.Message = "El plazo debe ser múltiplo de 6 entre 6 y 60 meses";
                    return result;
                }

                var totalDebt = await _loanRepository.GetAllQueryAsync()
                    .SelectMany(l => l.Installments!)
                    .Where(i => !i.IsPaid)
                    .SumAsync(i => i.PaymentAmount);

                var totalClients = await _clientApp.GetAllUser();

                var activeClientsCount = totalClients.Count(u => u.IsActive);

                var averageDebt = activeClientsCount > 0 ? totalDebt / activeClientsCount : 0;

                var clientDebt = await _loanRepository.GetAllQueryAsync()
                    .Where(l => l.UserId == client.Id)
                    .SelectMany(l => l.Installments!)
                    .Where(i => !i.IsPaid)
                    .SumAsync(i => i.PaymentAmount);

                var monthlyRate = dto.AnnualInterestRate / 12 / 100;
                var totalLoanWithInterest = dto.Amount * (1 + monthlyRate * dto.TermMonths);

                if (clientDebt > averageDebt)
                {
                    result.Message = "Este cliente se considera de alto riesgo, ya que su deuda actual supera el promedio";
                }
                else if ((clientDebt + totalLoanWithInterest) > averageDebt)
                {
                    result.Message = "Asignar este préstamo convertirá al cliente en alto riesgo";
                }

                var loan = new LoanDto
                {
                    Id = 0,
                    LoanNumber = GenerateLoanNumber(),
                    UserId = client.Id,
                    ApprovedByUserId = dto.ApprovedByUserId,
                    Amount = dto.Amount,
                    AnnualInterestRate = dto.AnnualInterestRate,
                    TermMonths = dto.TermMonths,
                    ApprovedAt = DateTime.UtcNow,
                    IsActive = true,
                    Installments = GenerateAmortizationTable(dto.Amount, monthlyRate, dto.TermMonths)
                };

                var newLoan = await base.AddAsync(loan);

                await _savingsAccountService.AddBalance(client.Id, dto.Amount);

                await _emailService.SendAsync(new EmailRequestDto
                {
                    To = client.Email,
                    Subject = "Préstamo aprobado - Artemis Banking",
                    HtmlBody = $@"
                                 <h3>Estimado {client.FirstName},</h3>
                                 <p>Su préstamo ha sido aprobado con los siguientes detalles:</p>
                                <ul>
                                    <li>Monto aprobado: {dto.Amount:C}</li>
                                    <li>Plazo: {dto.TermMonths} meses</li>
                                    <li>Tasa de interés anual: {dto.AnnualInterestRate}%</li>
                                    <li>Cuota mensual: {loan.Installments.First().PaymentAmount:C}</li>
                                </ul>
                                <p>Gracias por confiar en Artemis Banking.</p>"
                });

                result.IsError = false;
                result.Result = newLoan;
            }
            catch (Exception ex)
            {
                result.IsError = true;
                result.Message = ex.Message;
            }

            return result;
        }

        public async Task<ResultDto<List<ElegibleUserForLoanDto>>> GetElegibleUserForLoan()
        {
            var result = new ResultDto<List<ElegibleUserForLoanDto>>();
            try
            {
                var allUsers = await _clientApp.GetAllUser();

                if (allUsers == null || !allUsers.Any())
                {
                    result.IsError = true;
                    result.Message = "No hay usuarios registrados";
                    return result;
                }

                var activeUsers = allUsers.Where(u => u.IsActive).ToList();

                var elegibleUsers = new List<ElegibleUserForLoanDto>();

                foreach (var user in activeUsers)
                {
                    var hasActiveLoan = await _loanRepository.GetAllQueryAsync().AnyAsync(l => l.UserId == user.Id && l.IsActive);

                    if (!hasActiveLoan)
                    {
                        var mouthlyIncome = await _loanRepository.GetAllQueryAsync().Where(l => l.UserId == user.Id).SelectMany(l => l.Installments!).Where(i => !i.IsPaid).SumAsync(i => i.PaymentAmount);

                        var userFullName = $"{user.FirstName} {user.LastName}";

                        var dto = _mapper.Map<ElegibleUserForLoanDto>(user);
                        dto.FullName = userFullName;
                        dto.MonthlyIncome = mouthlyIncome;
                        elegibleUsers.Add(dto);
                    }
                }

                result.IsError = false;
                result.Result = elegibleUsers;
            }
            catch (Exception ex)
            {
                result.IsError = true;
                result.Message = ex.Message;
            }

            return result;
        }

        public async Task<ResultDto<ElegibleUserForLoanDto>> GetElegibleUserByIdentityForLoan(string identificationNumber)
        {
            var result = new ResultDto<ElegibleUserForLoanDto>();
            try
            {
                var user = await _clientApp.GetUserByIdentificationNumber(identificationNumber);

                if (user == null)
                {
                    result.IsError = true;
                    result.Message = "No se encontró el usuario";
                    return result;
                }

                var hasActiveLoan = await _loanRepository.GetAllQueryAsync()
                    .AnyAsync(l => l.UserId == user.Id && l.IsActive);

                if (hasActiveLoan)
                {
                    result.IsError = true;
                    result.Message = "El cliente ya tiene un préstamo activo";
                    return result;
                }

                var monthlyIncome = await _loanRepository.GetAllQueryAsync().Where(l => l.UserId == user.Id)
                    .SelectMany(l => l.Installments!).Where(i => !i.IsPaid).SumAsync(i => i.PaymentAmount);

                var dto = _mapper.Map<ElegibleUserForLoanDto>(user);
                dto.FullName = $"{user.FirstName} {user.LastName}";
                dto.MonthlyIncome = monthlyIncome;

                result.IsError = false;
                result.Result = dto;
            }
            catch (Exception ex)
            {
                result.IsError = true;
                result.Message = ex.Message;
            }

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

                var loansDto = _mapper.Map<List<LoanListDto>>(loansList);

              
                foreach (var loanDto in loansDto)
                {
                    loanDto.ClientFullName = $"{client.FirstName} {client.LastName}";
                }

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

        public async Task<ResultDto<LoanDetailDto>> GetLoanDetailAsync(int loanId)
        {
            var result = new ResultDto<LoanDetailDto>();
            try
            {
                var loan = await _loanRepository.GetQueryWithIncluide(new List<string>{ "Installments" })
                    .FirstOrDefaultAsync(l => l.Id == loanId);

                if (loan == null)
                {
                    result.IsError = true;
                    result.Message = "Préstamo no encontrado";
                    return result;
                }

                var client = await _clientApp.GetUserById(loan.UserId);

                if (client == null)
                {
                    result.IsError = true;
                    result.Message = "Cliente no encontrado";
                    return result;
                }

                var loanDetail = _mapper.Map<LoanDetailDto>(loan);

                loanDetail.ClientFullName = $"{client.FirstName} {client.LastName}";

                result.IsError = false;
                result.Result = loanDetail;
            }
            catch (Exception ex)
            {
                result.IsError = true;
                result.Message = ex.Message;
            }

            return result;
        }

        public async Task<ResultDto<LoanDto>> UpdateInterestRateAsync(int loanId, decimal newAnnualInterestRate)
        {
            var result = new ResultDto<LoanDto>();
            try
            {
                var loan = await _loanRepository.GetAllQueryAsync()
                    .Include(l => l.Installments)
                    .FirstOrDefaultAsync(l => l.Id == loanId);

                if (loan == null)
                {
                    result.IsError = true;
                    result.Message = "Préstamo no encontrado";
                    return result;
                }

                if (!loan.IsActive)
                {
                    result.IsError = true;
                    result.Message = "No se puede modificar un préstamo inactivo";
                    return result;
                }

                var client = await _clientApp.GetUserById(loan.UserId);

                if (client == null)
                {
                    result.IsError = true;
                    result.Message = "Cliente no encontrado";
                    return result;
                }

              
                var today = DateTime.UtcNow.Date;
                var futureInstallments = loan.Installments?
                    .Where(i => !i.IsPaid && i.PaymentDate.Date > today).OrderBy(i => i.PaymentDate)
                    .ToList();

                if (futureInstallments == null || !futureInstallments.Any())
                {
                    result.IsError = true;
                    result.Message = "No hay cuotas futuras para recalcular";
                    return result;
                }

                var paidInstallments = loan.Installments?.Count(i => i.IsPaid) ?? 0;
                var totalInstallments = loan.TermMonths;
                var remainingInstallments = futureInstallments.Count;

               
                var newMonthlyRate = newAnnualInterestRate / 12 / 100;

                var remainingCapital = CalculateRemainingCapital(
                    loan.Amount,
                    loan.AnnualInterestRate / 12 / 100,
                    totalInstallments,
                    paidInstallments
                );

                var pow = Pow(1 + newMonthlyRate, remainingInstallments);
                var newPaymentAmount = remainingCapital * (newMonthlyRate * pow) / (pow - 1);
                newPaymentAmount = Math.Round(newPaymentAmount, 2, MidpointRounding.AwayFromZero);

                foreach (var installment in futureInstallments)
                {
                    installment.PaymentAmount = newPaymentAmount;
                }

                loan.AnnualInterestRate = newAnnualInterestRate;

                await _loanRepository.UpdateAsync(loan.Id,loan);

                await _emailService.SendAsync(new EmailRequestDto
                {
                    To = client.Email,
                    Subject = "Actualización de tasa de interés - Artemis Banking",
                    HtmlBody = $@"
                    <h3>Estimado {client.FirstName},</h3>
                    <p>Le informamos que la tasa de interés de su préstamo ha sido actualizada.</p>
                    <ul>
                    <li>Nueva tasa de interés anual: {newAnnualInterestRate}%</li>
                    <li>Nueva cuota mensual: {newPaymentAmount:C}</li>
                    <li>Cuotas afectadas: {remainingInstallments}</li>
                    </ul>
                    <p>Esta actualización aplica únicamente a las cuotas pendientes de pago.</p>
                    <p>Gracias por confiar en Artemis Banking.</p>"
                });

                var updatedLoan = _mapper.Map<LoanDto>(loan);

                result.IsError = false;
                result.Result = updatedLoan;
                result.Message = "Tasa de interés actualizada exitosamente";
            }
            catch (Exception ex)
            {
                result.IsError = true;
                result.Message = ex.Message;
            }

            return result;
        }

     
        #region Private Methods 
        private string GenerateLoanNumber()
        {
            var random = new Random();
            return random.Next(100000000, 999999999).ToString();
        }

        private List<InstallmentDto> GenerateAmortizationTable(decimal P, decimal monthlyRate, int n)
        {
            var installments = new List<InstallmentDto>();

            var pow = Pow(1 + monthlyRate, n);

            var C = P * (monthlyRate * pow) / (pow - 1);

            for (int i = 1; i <= n; i++)
            {
                installments.Add(new InstallmentDto
                {
                    Id = 0,
                    PaymentDate = DateTime.UtcNow.AddMonths(i),
                    PaymentAmount = Math.Round(C, 2, MidpointRounding.AwayFromZero),
                    IsPaid = false,
                    IsLate = false
                });
            }

            return installments;
        }

        private decimal Pow(decimal baseValue, int exponent)
        {
            decimal result = 1;
            for (int i = 0; i < exponent; i++)
            {
                result *= baseValue;
            }
            return result;
        }

        private decimal CalculateRemainingCapital(decimal initialCapital, decimal monthlyRate, int totalInstallments, int paidInstallments)
        {
            if (paidInstallments == 0)
                return initialCapital;

            var pow = Pow(1 + monthlyRate, totalInstallments);
            var monthlyPayment = initialCapital * (monthlyRate * pow) / (pow - 1);

            decimal remainingCapital = initialCapital;

            for (int i = 1; i <= paidInstallments; i++)
            {
                var interest = remainingCapital * monthlyRate;
                var principal = monthlyPayment - interest;
                remainingCapital -= principal;
            }

            return remainingCapital;
        }

        #endregion

    }
}
