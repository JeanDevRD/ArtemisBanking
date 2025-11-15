// BeneficiaryService.cs → REEMPLAZAR TODO EL ARCHIVO
using ArtemisBanking.Core.Application.Dtos.Beneficiary;
using ArtemisBanking.Core.Application.Interfaces;
using ArtemisBanking.Core.Domain.Entities;
using ArtemisBanking.Core.Domain.Interfaces;
using AutoMapper;

namespace ArtemisBanking.Core.Application.Services
{
    public class BeneficiaryService : GenericService<BeneficiaryDto, Beneficiary>, IBeneficiaryService
    {
        private readonly IBeneficiaryRepository _beneficiaryRepository; // ← AÑADIDO
        private readonly ITransactionService _transactionService;

        public BeneficiaryService(
            IBeneficiaryRepository beneficiaryRepository,
            ITransactionService transactionService,
            IMapper mapper
        ) : base(beneficiaryRepository, mapper)
        {
            _beneficiaryRepository = beneficiaryRepository;
            _transactionService = transactionService;
        }

        public async Task<bool> ExistsByAccountNumberAsync(string accountNumber)
        {
            var all = await GetAllAsync();
            return all.Any(b => b.AccountNumber == accountNumber);
        }

        public async Task<bool> ExistsByCedulaAsync(string cedula)
        {
            if (!IsValidDominicanCedula(cedula))
                return false;
            var all = await GetAllAsync();
            return all.Any(b => b.Cedula == cedula);
        }

        private bool IsValidDominicanCedula(string cedula)
        {
            cedula = cedula.Replace("-", "");
            if (cedula.Length != 11 || !cedula.All(char.IsDigit)) return false;
            var digits = cedula.Select(c => c - '0').ToArray();
            var checkDigit = digits[10];
            var sum = 0;
            for (int i = 0; i < 10; i++)
            {
                var num = digits[i];
                if (i % 2 == 0) num *= 2;
                if (num > 9) num -= 9;
                sum += num;
            }
            var calculated = (10 - (sum % 10)) % 10;
            return calculated == checkDigit;
        }

        public async Task<bool> HasPendingTransactionsAsync(int beneficiaryId)
        {
            var allTx = await _transactionService.GetAllAsync();
            return allTx.Any(t => t.Beneficiary == beneficiaryId.ToString() && t.Status == 0);
        }
    }
}