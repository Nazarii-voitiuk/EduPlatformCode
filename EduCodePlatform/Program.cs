using EduCodePlatform.Data;
using Microsoft.EntityFrameworkCore;
using EduCodePlatform.Data;

var builder = WebApplication.CreateBuilder(args);

// Додаємо сервіси до контейнера
builder.Services.AddControllersWithViews();

// Реєструємо контекст бази даних123123123
builder.Services.AddDbContext<ApplicationDbContext>(options =>
    options.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection")));

var app = builder.Build();

// Налаштування конвеєра обробки запитів
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
