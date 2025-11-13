using ArtemisBanking.Infraestructure.Identity;
using ArtemisBanking.Infraestructure.Shared;
using ArtemisBanking.Infraestructure.Persistence;
using ArtemisBanking.Core.Application;


var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddControllersWithViews();

builder.Services.AddSingleton<IHttpContextAccessor, HttpContextAccessor>();

builder.Services.AddPersistenceLayer(builder.Configuration);  
builder.Services.AddSharedLayer(builder.Configuration);        
builder.Services.AddApplicationLayer();                        
builder.Services.AddIdentityLayerForWebApp(builder.Configuration);


<<<<<<< HEAD




=======
>>>>>>> 7d98239b2142fae3e55a75da7d703e6d3084a0f9
builder.Services.AddSession(opt =>
{
    opt.IdleTimeout = TimeSpan.FromMinutes(60);
    opt.Cookie.HttpOnly = true;
    opt.Cookie.IsEssential = true;
});

var app = builder.Build();

await app.Services.RunIdentitySeedAsync();

// Configure the HTTP request pipeline.
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Shared/ErrorGeneral");
    app.UseHsts();
}


app.UseHttpsRedirection();
app.UseStaticFiles();

app.UseRouting();
app.UseSession();              // 
app.UseAuthentication(); // <-- Añadida



app.UseAuthorization();

app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=Index}/{id?}");

app.Run();
