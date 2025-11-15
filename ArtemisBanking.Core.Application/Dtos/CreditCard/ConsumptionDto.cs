namespace ArtemisBanking.Core.Application.Dtos.CreditCard
{
    public class ConsumptionDto
    {
        public required DateTime consumptionDate; 
        public required decimal amount;             
        public required string merchant;               
        public required int status;
    }
}
