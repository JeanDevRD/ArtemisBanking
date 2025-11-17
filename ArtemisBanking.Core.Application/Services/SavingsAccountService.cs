using ArtemisBanking.Core.Application.Dtos.Common;
using ArtemisBanking.Core.Application.Dtos.SavingsAccount;
using ArtemisBanking.Core.Application.Interfaces;
using ArtemisBanking.Core.Domain.Common.Enum;
using ArtemisBanking.Core.Domain.Entities;
using ArtemisBanking.Infraestructure.Persistence.Repositories;
using AutoMapper;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;

namespace ArtemisBanking.Core.Application.Services
{
    public class SavingsAccountService : GenericService<SavingsAccountDto, SavingsAccount>, ISavingsAccountService
    {
        private readonly ISavingsAccountRepository _savingsAccountRepository;
        private readonly IMapper _mapper;
        private readonly IServiceProvider _serviceProvider;
        public SavingsAccountService(ISavingsAccountRepository savingsAccountRepository, IMapper mapper, IServiceProvider serviceProvider) : base(savingsAccountRepository, mapper)
        {
            _savingsAccountRepository = savingsAccountRepository;
            _mapper = mapper;
            _serviceProvider = serviceProvider;
        }
        public override async Task<SavingsAccountDto> GetByIdAsync(int id)
        {
            try
            {
                var entities = await _savingsAccountRepository.GetAllListIncluideAsync(["Transactions"]);

                var entity = entities.FirstOrDefault(e => e.Id == id);
                if (entity == null)
                {
                    return null!;
                }
                return _mapper.Map<SavingsAccountDto>(entity);
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
                return null!;
            }
        }

        public async Task<List<SavingsAccountDto>> GetAllWithInclude()
        {
            try
            {
                var creditCards = await _savingsAccountRepository.GetAllListIncluideAsync(["Transactions"]);
                if (creditCards == null)
                {
                    return new List<SavingsAccountDto>();
                }
                return _mapper.Map<List<SavingsAccountDto>>(creditCards);
            }
            catch (Exception ex)
            {
                throw new Exception("Error retrieving credit cards with included data: " + ex.Message);
            }
        }

        public async Task AddBalance(string userId, decimal amount)
        {
            var account = await _savingsAccountRepository.GetAllQueryAsync().FirstOrDefaultAsync(a => a.UserId == userId && a.Type == (int)TypeSavingAccount.Main);

            if (account == null)
                throw new Exception("Cuenta principal no encontrada");

            account.Balance += amount;

            await _savingsAccountRepository.UpdateAsync(account.Id, account);
        }

        public async Task<ResultDto<List<SavingsAccountsHomeDto>>> GetSavingAccountHome(string? identificationNumber, int page, bool? isActive = null, int? accountType = null)
        {
            var result = new ResultDto<List<SavingsAccountsHomeDto>>();
            var clientService = _serviceProvider.GetRequiredService<IAccountServiceForApi>();

            try
            {
                var query = _savingsAccountRepository.GetAllQueryAsync();

                if (!string.IsNullOrWhiteSpace(identificationNumber))
                {
                    var client = await clientService.GetUserByIdentificationNumber(identificationNumber);
                    if (client == null)
                    {
                        result.IsError = true;
                        result.Message = "Cliente no encontrado";
                        return result;
                    }
                    query = query.Where(a => a.UserId == client.Id);
                }

            
                if (isActive.HasValue)
                {
                    query = query.Where(a => a.IsActive == isActive.Value);
                }

                if (accountType.HasValue)
                {
                    query = query.Where(a => a.Type == accountType.Value);
                }

                var accounts = await query.OrderByDescending(a => a.IsActive)
                    .ThenByDescending(a => a.CreatedAt).ToListAsync();

                if (!accounts.Any())
                {
                    result.IsError = true;
                    result.Message = "No se encontraron cuentas de ahorro";
                    return result;
                }

                var totalCount = accounts.Count;

                var pagedAccounts = accounts.Skip((page - 1) * 20).Take(20).ToList();

                var accountsResult = new List<SavingsAccountsHomeDto>();
                foreach (var account in pagedAccounts)
                {
                    var client = await clientService.GetUserById(account.UserId);
                    if (client != null)
                    {
                        accountsResult.Add(new SavingsAccountsHomeDto
                        {
                            AccountId = account.Id,
                            AccountNumber = account.AccountNumber,
                            UserFullName = $"{client.FirstName} {client.LastName}",
                            Balance = account.Balance,
                            AccountType = account.Type == 1 ? "Principal" : "Secundaria",
                            IsActive = account.IsActive,
                            IsPrincipal = account.Type == 1
                        });
                    }
                }

                result.IsError = false;
                result.Result = accountsResult;
            }
            catch (Exception ex)
            {
                result.IsError = true;
                result.Message = $"Error al obtener las cuentas: {ex.Message}";
            }

            return result;
        }

        public async Task<ResultDto<SavingsAccountDto>> AddSecondarySavingsAccount(CreateSecundarySavingsAccountsDto dto)
        {
            var result = new ResultDto<SavingsAccountDto>();
            var clientService = _serviceProvider.GetRequiredService<IAccountServiceForApi>();

            try
            {
                var client = await clientService.GetUserById(dto.UserId);
                if (client == null || !client.IsActive)
                {
                    result.IsError = true;
                    result.Message = "Cliente no válido o inactivo";
                    return result;
                }

                string accountNumber;
                bool isUnique;
                do
                {
                    accountNumber = GenerateAccountNumber();
                    isUnique = !await _savingsAccountRepository.GetAllQueryAsync()
                        .AnyAsync(a => a.AccountNumber == accountNumber);
                } while (!isUnique);

                var newAccount = new SavingsAccountDto
                {
                    Id = 0,
                    AccountNumber = accountNumber,
                    UserId = dto.UserId,
                    Balance = dto.InitialBalance,
                    Type = (int)TypeSavingAccount.Secondary, 
                    IsActive = true,
                    CreatedAt = DateTime.UtcNow,
                    AssignedByUserId = dto.AdminUserId
                };

                var createdAccount = await base.AddAsync(newAccount);

                result.IsError = false;
                result.Result = createdAccount;
                result.Message = "Cuenta secundaria creada exitosamente";
            }
            catch (Exception ex)
            {
                result.IsError = true;
                result.Message = $"Error al crear la cuenta: {ex.Message}";
            }

            return result;
        }

        public async Task<ResultDto<SavingsAccountDetailDto>> GetSavingsAccountDetail(string accountId)
        {
            var result = new ResultDto<SavingsAccountDetailDto>();
            var clientService = _serviceProvider.GetRequiredService<IAccountServiceForApi>();

            try
            {
                var account = await _savingsAccountRepository.GetAllQueryAsync()
                    .Include(a => a.Transactions)
                    .FirstOrDefaultAsync(a => a.AccountNumber == accountId);

                if (account == null)
                {
                    result.IsError = true;
                    result.Message = "Cuenta no encontrada";
                    return result;
                }

                var client = await clientService.GetUserById(account.UserId);
                if (client == null)
                {
                    result.IsError = true;
                    result.Message = "Cliente no encontrado";
                    return result;
                }

                var detail = new SavingsAccountDetailDto
                {
                    AccountId = account.Id,
                    AccountNumber = account.AccountNumber,
                    ClientFullName = $"{client.FirstName} {client.LastName}",
                    Balance = account.Balance,
                    AccountType = account.Type == 1 ? "Principal" : "Secundaria",
                    Transactions = account.Transactions?
                        .OrderByDescending(t => t.Date)
                        .Select(t => new TransactionDetailDto
                        {
                            TransactionId = t.Id,
                            TransactionDate = t.Date,
                            Amount = t.Amount,
                            TransactionType = t.Type == 1 ? "DÉBITO" : "CRÉDITO",
                            Beneficiary = t.Beneficiary,
                            Origin = t.Source,
                            Status = t.Status == 1 ? "APROBADA" : "RECHAZADA"
                        }).ToList() ?? new List<TransactionDetailDto>()
                };

                result.IsError = false;
                result.Result = detail;
            }
            catch (Exception ex)
            {
                result.IsError = true;
                result.Message = $"Error al obtener detalle: {ex.Message}";
            }

            return result;
        }

        public async Task<ResultDto<SavingsAccountDto>> CancelSecondarySavingsAccount(string accountId)
        {
            var result = new ResultDto<SavingsAccountDto>();

            try
            {
                var account = await _savingsAccountRepository.GetAllQueryAsync()
                    .FirstOrDefaultAsync(a => a.AccountNumber == accountId);

                if (account == null)
                {
                    result.IsError = true;
                    result.Message = "Cuenta no encontrada";
                    return result;
                }

                
                if (account.Type == (int)TypeSavingAccount.Main)
                {
                    result.IsError = true;
                    result.Message = "No se puede cancelar una cuenta principal";
                    return result;
                }

                if (!account.IsActive)
                {
                    result.IsError = true;
                    result.Message = "La cuenta ya está cancelada";
                    return result;
                }

                
                if (account.Balance > 0)
                {
                    var principalAccount = await _savingsAccountRepository.GetAllQueryAsync().FirstOrDefaultAsync(a => a.UserId == account.UserId 
                    && a.Type == (int)TypeSavingAccount.Main && a.IsActive);

                    if (principalAccount == null)
                    {
                        result.IsError = true;
                        result.Message = "No se encontró cuenta principal para transferir el balance";
                        return result;
                    }

                    principalAccount.Balance += account.Balance;
                    await _savingsAccountRepository.UpdateAsync(principalAccount.Id, principalAccount);

                    account.Balance = 0;
                }

                account.IsActive = false;
                await _savingsAccountRepository.UpdateAsync(account.Id, account);

                var canceledAccount = _mapper.Map<SavingsAccountDto>(account);

                result.IsError = false;
                result.Result = canceledAccount;
                result.Message = "Cuenta cancelada exitosamente";
            }
            catch (Exception ex)
            {
                result.IsError = true;
                result.Message = $"Error al cancelar la cuenta: {ex.Message}";
            }

            return result;
        }

        #region Private Methods
        private string GenerateAccountNumber()
        {
            var random = new Random();
            return random.Next(100000000, 999999999).ToString();
        }
       
        #endregion



    }
}
