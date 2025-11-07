using ArtemisBanking.Core.Application.Dtos.CardTransaction;
using ArtemisBanking.Core.Application.Dtos.CreditCard;
using ArtemisBanking.Core.Application.Dtos.SavingsAccount;


namespace ArtemisBanking.Core.Application.Interfaces
{
    public interface ICreditCardService : IGenericService<CreditCardDto>
    {
        Task<List<CreditCardDto>> GetAllWithInclude();
    }
}
