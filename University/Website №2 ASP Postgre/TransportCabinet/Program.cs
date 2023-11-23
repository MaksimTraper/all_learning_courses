using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.EntityFrameworkCore;
using System.Configuration;
using TransportCabinet.Data;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddControllersWithViews(); // ��������� ������� MVC
string connection = builder.Configuration.GetConnectionString("DefaultConnection");
builder.Services.AddDbContext<TransportCabinetContext>(options => options.UseNpgsql(connection));

// �������������� � ������� ����
builder.Services.AddAuthentication(CookieAuthenticationDefaults.AuthenticationScheme)
	.AddCookie(options => options.LoginPath = "/Unavailable");
builder.Services.AddAuthorization();
builder.Services.AddSession();
builder.Services.AddMemoryCache();

var app = builder.Build();

app.UseAuthentication();   // ���������� middleware �������������� 
app.UseAuthorization();   // ���������� middleware �����������

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