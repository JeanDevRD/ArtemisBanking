using ArtemisBanking.Core.Application.Interfaces;
using ArtemisBanking.Core.Domain.Settings;
using ArtemisBanking.Infraestructure.Shared.Services;
using AutoMapper;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace ArtemisBanking.Infraestructure.Shared
{
    public static class ServiceRegistration
    {
        public static void AddSheredLayer(this IServiceCollection service, IConfiguration config)
        {
            #region Configurations
            service.Configure<MailSettings>(config.GetSection("MailSettings"));
            #endregion

            #region Services IOC
            service.AddScoped<IEmailService, EmailService>();
            #endregion

        }
    }
}
