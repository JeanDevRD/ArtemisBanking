using ArtemisBanking.Core.Application.Dtos.SavingsAccount;
using ArtemisBanking.Core.Application.Interfaces;
using ArtemisBanking.Core.Domain.Entities;
using ArtemisBanking.Infraestructure.Persistence.Repositories;
using AutoMapper;

namespace ArtemisBanking.Core.Application.Services
{
    public class SavingsAccountService : GenericService<SavingsAccountDto, SavingsAccount>, ISavingsAccountService
    {
        private readonly ISavingsAccountRepository _savingsAccountRepository;
        private readonly IMapper _mapper;
        public SavingsAccountService(ISavingsAccountRepository savingsAccountRepository, IMapper mapper) : base(savingsAccountRepository, mapper)
        {
            _savingsAccountRepository = savingsAccountRepository;
            _mapper = mapper;
        }
    }
}
