using ArtemisBanking.Core.Application.Dtos.Common;
using ArtemisBanking.Core.Application.Dtos.CreditCard;
using ArtemisBanking.Core.Application.Dtos.Loan;


namespace ArtemisBanking.Core.Application.Interfaces
{
    public interface ICreditCardService : IGenericService<CreditCardDto>
    {
        Task<List<CreditCardDto>> GetAllWithInclude();
        Task<ResultDto<List<CreditCardListDto>>> GetAllActiveCreditCard();
        Task<ResultDto<List<CreditCardListDto>>> GetAllCreditCardByIdentityUser(string identificationNumber, bool? isActive = null);
        Task<bool> CancelatedCreditCard(string creditCardId);
        Task<ResultDto<List<ElegibleUserForCreditCardDto>>> GetElegibleUserForCreditCard();
        Task<ResultDto<ElegibleUserForCreditCardDto>> GetElegibleUserByIdentityForCreditCard(string identificationNumber);
        Task<ResultDto<CreditCardDto>> AddCreditCardAsync(CreditCardRequestDto request, string adminUserId);
        Task<ResultDto<CreditCardDto>> UpdateCard(UpdateCreditCardDto dto, string creditCardId);
        Task<ResultDto<CreditCardDetailDto>> DetailCard(string idCreditCard);
    }
}
