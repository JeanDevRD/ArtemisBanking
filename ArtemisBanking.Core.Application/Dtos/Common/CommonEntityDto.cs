namespace ArtemisBanking.Core.Application.Dtos.Common
{
    public class CommonEntityDto<Tkey>
    {
        public required Tkey Id { get; set; }
    }
}
