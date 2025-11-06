using ArtemisBanking.Core.Application.Dtos.Transaction;
using ArtemisBanking.Core.Application.Interfaces;
using ArtemisBanking.Core.Domain.Entities;
using ArtemisBanking.Infraestructure.Persistence.Repositories;
using AutoMapper;


namespace ArtemisBanking.Core.Application.Services
{
    public class TransactionService : GenericService<TransactionDto, Transaction>, ITransactionService
    {
        private readonly ITransactionRepository _transactionRepository;
        private readonly IMapper _mapper;
        public TransactionService(ITransactionRepository transactionRepository, IMapper mapper): base(transactionRepository , mapper)
        {
            _transactionRepository = transactionRepository;
            _mapper = mapper;
        }
    }
}
