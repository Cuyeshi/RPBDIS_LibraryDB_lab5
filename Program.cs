using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using RPBDIS_LibraryDB_lab5.Data;
using RPBDIS_LibraryDB_lab5.Models; // ���������, ��� ���� namespace ������������� ������ �������


var builder = WebApplication.CreateBuilder(args);

// ��������� ������� ��� ������ � ������������� � ���������������
builder.Services.AddControllersWithViews();

// ������������ �������� ���� ������ (�������� ������ ����������� �� ����)
builder.Services.AddDbContext<LibraryDbContext>(options =>
    options.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection")));

// Регистрация ApplicationDbContext для Identity
builder.Services.AddDbContext<RPBDIS_LibraryDB_lab5Context>(options =>
    options.UseSqlServer(builder.Configuration.GetConnectionString("RPBDIS_LibraryDB_lab5ContextConnection")));

builder.Services.AddDefaultIdentity<IdentityUser>(options =>
{
    options.SignIn.RequireConfirmedAccount = false;
    options.Password.RequireDigit = false;
    options.Password.RequiredLength = 6;
    options.Password.RequireNonAlphanumeric = false;
    options.Password.RequireUppercase = false;
    options.Password.RequireLowercase = false;
})
.AddEntityFrameworkStores<RPBDIS_LibraryDB_lab5Context>();




builder.Services.ConfigureApplicationCookie(options =>
{
    options.LoginPath = "/Account/Login"; // Путь для перенаправления на страницу входа
    options.AccessDeniedPath = "/Account/AccessDenied"; // Путь для страницы с ограничением доступа
});

//builder.Services.AddRazorPages(options =>
//{
//    options.Conventions.AllowAnonymousToPage("/Identity/Account/Login");
//    options.Conventions.AllowAnonymousToPage("/Identity/Account/Register");
//    options.Conventions.AllowAnonymousToPage("/Identity/Account/AccessDenied");
//});

//builder.Services.AddControllersWithViews(options =>
//{
//    // Добавляем глобальную политику авторизации
//    var policy = new AuthorizationPolicyBuilder()
//        .RequireAuthenticatedUser()
//        .Build();
//    options.Filters.Add(new Microsoft.AspNetCore.Mvc.Authorization.AuthorizeFilter(policy));
//});

// Логирование
builder.Logging.ClearProviders();
builder.Logging.AddConsole();
builder.Logging.AddDebug();

builder.Services.AddRazorPages();
//////////////////////////////////////////////////////////////////////////////////////

var app = builder.Build();

// Configure the HTTP request pipeline.
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Home/Error");
    app.UseHsts();
}

app.UseHttpsRedirection();
app.UseStaticFiles();

app.UseRouting();

app.UseAuthentication();
app.UseAuthorization();

//app.Use(async (context, next) =>
//{
//    if (string.IsNullOrEmpty(context.Request.Path) || context.Request.Path == "/")
//    {
//        context.Response.Redirect("/Identity/Account/Login");
//        return;
//    }
//    await next();
//});

app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=Index}/{id?}");
app.MapRazorPages();

app.Run();