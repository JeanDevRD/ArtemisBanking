using ArtemisBanking.Core.Application.Dtos.Beneficiary;
using ArtemisBanking.Core.Application.Interfaces;
using ArtemisBanking.Core.Domain.Entities;
using ArtemisBanking.Core.Domain.Interfaces;
using AutoMapper;

namespace ArtemisBanking.Core.Application.Services
{
    public class BeneficiaryService : GenericService<BeneficiaryDto, Beneficiary>, IBeneficiaryService
    {
        private readonly ITransactionService _transactionService;

        public BeneficiaryService(IBeneficiaryRepository beneficiaryRepository,ITransactionService transactionService,
            IMapper mapper
        ) : base(beneficiaryRepository, mapper)
        {
            _transactionService = transactionService;
        }

        public async Task<bool> ExistsByAccountNumberAsync(string accountNumber)
        {
            var all = await GetAllAsync();
            return all.Any(b => b.AccountNumber == accountNumber);
        }

        public async Task<bool> ExistsByCedulaAsync(string cedula)
        {
            var all = await GetAllAsync();
            return all.Any(b => b.UserId == cedula);
        }

        public async Task<bool> HasPendingTransactionsAsync(int beneficiaryId)
        {
            var allTx = await _transactionService.GetAllAsync();
            // Comprobamos si hay alguna transacción con el beneficiario y estado pendiente (0)
            return allTx.Any(t => t.Beneficiary == beneficiaryId.ToString() && t.Status == 0);
        }
    }
}


