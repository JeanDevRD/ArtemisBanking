namespace ArtemisBanking.Core.Application.Dtos.AdminDashboard
{
    public class TotalClientsDto
    {
        public required int TotalClientsActive { get; set; }
        public required int TotalClientsInactive { get; set; }
    }
}
