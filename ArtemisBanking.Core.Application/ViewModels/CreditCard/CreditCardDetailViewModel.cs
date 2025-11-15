

namespace ArtemisBanking.Core.Application.ViewModel.CreditCard
{
    public class CreditCardDetailViewModel
    {
        public required string cardId;                
        public required string cardNumberMasked;       
        public List<ConsumptionViewModel>? consumptions;
    }
}
