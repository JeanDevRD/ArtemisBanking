namespace ArtemisBanking.Core.Application.Dtos.Common
{
    public class ResultDto<T> where T : class
    {
        public bool IsError { get; set; } = false;
        public string? Message { get; set; }
        public T? Result { get; set; }
    }
}
