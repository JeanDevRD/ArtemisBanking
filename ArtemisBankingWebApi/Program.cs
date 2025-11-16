using ArtemisBanking.Core.Application;
using ArtemisBanking.Core.Application.Interfaces;
using ArtemisBanking.Infraestructure.Identity;
using ArtemisBanking.Infraestructure.Identity.Services;
using ArtemisBanking.Infraestructure.Persistence;
using ArtemisBanking.Infraestructure.Shared;
using ArtemisBankingWebApi.Extensions;
using System.Text.Json.Serialization;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddControllers()
    .AddJsonOptions(options =>
    {
        options.JsonSerializerOptions.ReferenceHandler = ReferenceHandler.IgnoreCycles;
        options.JsonSerializerOptions.Converters.Add(new JsonStringEnumConverter());
    });

builder.Services.AddPersistenceLayer(builder.Configuration);
builder.Services.AddApplicationLayer();
builder.Services.AddSharedLayer(builder.Configuration);
builder.Services.AddIdentityLayerForWebApi(builder.Configuration);
builder.Services.AddScoped<IAccountServiceForApp, AccountServiceForApp>(); 

builder.Services.AddEndpointsApiExplorer();
builder.Services.AddHealthChecks();
builder.Services.AddApiVersioningConfiguration();
builder.Services.AddSwaggerConfiguration();

builder.Services.AddDistributedMemoryCache();
builder.Services.AddSession();

builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowAll", policy =>
    {
        policy.AllowAnyOrigin()
              .AllowAnyMethod()
              .AllowAnyHeader();
    });
});

var app = builder.Build();

// Run Identity Seed
await app.Services.RunIdentitySeedAsync();

// Configure the HTTP request pipeline
if (app.Environment.IsDevelopment())
{
    app.UseSwaggerConfiguration();
}

app.UseHttpsRedirection();
app.UseCors("AllowAll");
app.UseAuthentication();
app.UseAuthorization();
app.UseHealthChecks("/health");
app.UseSession();

app.MapControllers();

await app.RunAsync();