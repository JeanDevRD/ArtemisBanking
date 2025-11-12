using ArtemisBanking.Core.Application.Interfaces;
using ArtemisBanking.Core.Domain.Settings;
using ArtemisBanking.Infraestructure.Shared.Services;
using AutoMapper;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;

namespace ArtemisBanking.Infraestructure.Shared
{
    public static class ServiceRegistration
    {
        public static void AddSharedLayer(this IServiceCollection service, IConfiguration config)
        {
            #region Configurations
            service.Configure<MailSettings>(config.GetSection("MailSettings"));
            service.AddSingleton(sp => sp.GetRequiredService<IOptions<MailSettings>>().Value);
            #endregion

            #region Services IOC
            service.AddScoped<IEmailService, EmailService>();
            #endregion

        }
    }
}
