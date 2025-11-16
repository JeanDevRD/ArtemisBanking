namespace ArtemisBanking.Core.Application.ViewModel.CreditCard
{
    public class ConsumptionViewModel
    {
        public required DateTime consumptionDate; 
        public required decimal amount;             
        public required string merchant;               
        public required int status;
    }
}
