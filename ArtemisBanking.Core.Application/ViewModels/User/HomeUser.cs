namespace ArtemisBanking.Core.Application.ViewModels.User
{
    public class HomeUser
    {
        public List<UserViewModel> Users { get; set; } = new List<UserViewModel>();
        public int PageNumber { get; set; }
        public int TotalPages { get; set; }
    }
}
