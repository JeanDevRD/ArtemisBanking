using ArtemisBanking.Core.Application.ViewModel.CreditCard;

namespace ArtemisBanking.Core.Application.ViewModels.CreditCard
{
    public class HomeCreditCardViewModel
    {
        public List<CreditCardListViewModel> Cards { get; set; } = new();

        public int PageNumber { get; set; }
        public int TotalPages { get; set; }

        public string? FilterIdentificationNumber { get; set; } 
        public bool? IsActive { get; set; }
    }
}
