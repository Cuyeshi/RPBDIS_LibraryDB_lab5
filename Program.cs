using Microsoft.EntityFrameworkCore;
using RPBDIS_LibraryDB_lab5.Models; // ”бедитесь, что этот namespace соответствует вашему проекту


var builder = WebApplication.CreateBuilder(args);

// ƒобавл€ем сервисы дл€ работы с контроллерами и представлени€ми
builder.Services.AddControllersWithViews();

// –егистрируем контекст базы данных (замените строку подключени€ на вашу)
builder.Services.AddDbContext<LibraryDbContext>(options =>
    options.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection")));


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

app.UseAuthorization();


app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=Index}/{id?}");

app.Run();
