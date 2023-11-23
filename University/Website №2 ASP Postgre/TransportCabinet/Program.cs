using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.EntityFrameworkCore;
using System.Configuration;
using TransportCabinet.Data;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddControllersWithViews(); // добавляем сервисы MVC
string connection = builder.Configuration.GetConnectionString("DefaultConnection");
builder.Services.AddDbContext<TransportCabinetContext>(options => options.UseNpgsql(connection));

// аутентификация с помощью куки
builder.Services.AddAuthentication(CookieAuthenticationDefaults.AuthenticationScheme)
	.AddCookie(options => options.LoginPath = "/Unavailable");
builder.Services.AddAuthorization();
builder.Services.AddSession();
builder.Services.AddMemoryCache();

var app = builder.Build();

app.UseAuthentication();   // добавление middleware аутентификации 
app.UseAuthorization();   // добавление middleware авторизации

app.UseStaticFiles(new StaticFileOptions()
{
    OnPrepareResponse = ctx =>
    {
        ctx.Context.Response.Headers.Add("Cache-Control", "public,max-age=1");
    }
});

//app.MapControllerRoute(
//    name: "default",
//    pattern: "{controller=Auth}/{action=SignupForm}/{id?}");

app.MapControllerRoute(
	name: "default",
	pattern: "{controller=Home}/{action=StartPage}/{id?}");


app.Run();