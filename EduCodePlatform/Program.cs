using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using EduCodePlatform.Data;
using EduCodePlatform.Models.Identity; // ваш клас ApplicationUser

var builder = WebApplication.CreateBuilder(args);

// Підключення БД
builder.Services.AddDbContext<ApplicationDbContext>(options =>
    options.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection")));

// Identity з вашим ApplicationUser : IdentityUser
builder.Services.AddDefaultIdentity<ApplicationUser>(options =>
{
    options.Password.RequireDigit = false;
    options.Password.RequireNonAlphanumeric = false;
    options.Password.RequiredLength = 6;
})
.AddEntityFrameworkStores<ApplicationDbContext>();

// Можливо, якщо плануєте використовувати сторінки Razor Area Identity
builder.Services.AddRazorPages();

// Можна також .AddControllersWithViews()
builder.Services.AddControllersWithViews();

var app = builder.Build();

if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Home/Error");
    app.UseHsts();
}

app.UseHttpsRedirection();
app.UseStaticFiles();

app.UseRouting();

// Identity
app.UseAuthentication();
app.UseAuthorization();

// Маршрути
app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=Index}/{id?}"
);

// Для Razor Pages Identity
app.MapRazorPages();

app.Run();
