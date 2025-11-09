using ArtemisBanking.Core.Application.Dtos.Email;

namespace ArtemisBanking.Core.Application.Interfaces
{
    public interface IEmailService
    {
        Task SendAsync(EmailRequestDto emailRequestDto);
    }
}