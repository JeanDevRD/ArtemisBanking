using ArtemisBanking.Infraestructure.Identity;
using ArtemisBanking.Infraestructure.Shared;
using ArtemisBanking.Infraestructure.Persistence;
using ArtemisBanking.Core.Application;


var builder = WebApplication.CreateBuilder(args);

builder.Services.AddControllersWithViews();

builder.Services.AddSingleton<IHttpContextAccessor, HttpContextAccessor>();

builder.Services.AddPersistenceLayer(builder.Configuration);  
builder.Services.AddSharedLayer(builder.Configuration);        
builder.Services.AddApplicationLayer();                        
builder.Services.AddIdentityLayerForWebApp(builder.Configuration);



builder.Services.AddSession(opt =>
{
    opt.IdleTimeout = TimeSpan.FromMinutes(60);
    opt.Cookie.HttpOnly = true;
    opt.Cookie.IsEssential = true;
});

var app = builder.Build();

await app.Services.RunIdentitySeedAsync();

if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Shared/ErrorGeneral");
    app.UseHsts();
}


app.UseHttpsRedirection();
app.UseStaticFiles();

app.UseRouting();
app.UseSession(); 
app.UseAuthentication(); 
app.UseAuthorization();




app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Auth}/{action=Login}/{id?}");

app.Run();
