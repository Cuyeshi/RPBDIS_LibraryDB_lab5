using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using RPBDIS_LibraryDB_lab5.Data;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddControllersWithViews();

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
.AddRoles<IdentityRole>()
.AddEntityFrameworkStores<RPBDIS_LibraryDB_lab5Context>();

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

app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=Index}/{id?}");
app.MapControllerRoute(
    name: "search",
    pattern: "LoanedBooks/SearchBooks");
app.MapRazorPages();

app.Run();