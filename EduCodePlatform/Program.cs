using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using EduCodePlatform.Data;
using EduCodePlatform.Models.Identity; // âàø êëàñ ApplicationUser

var builder = WebApplication.CreateBuilder(args);

// Äîäàºìî ñåðâ³ñè äî êîíòåéíåðà
builder.Services.AddControllersWithViews();

// Ðåºñòðóºìî êîíòåêñò áàçè äàíèõ123123123
builder.Services.AddDbContext<ApplicationDbContext>(options =>
    options.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection")));

// Identity ç âàøèì ApplicationUser : IdentityUser
builder.Services.AddDefaultIdentity<ApplicationUser>(options =>
{
    options.Password.RequireDigit = false;
    options.Password.RequireNonAlphanumeric = false;
    options.Password.RequiredLength = 6;
})
.AddEntityFrameworkStores<ApplicationDbContext>();

// Ìîæëèâî, ÿêùî ïëàíóºòå âèêîðèñòîâóâàòè ñòîð³íêè Razor Area Identity
builder.Services.AddRazorPages();

// Ìîæíà òàêîæ .AddControllersWithViews()
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

// Ìàðøðóòè
app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=Index}/{id?}"
);

// Äëÿ Razor Pages Identity
app.MapRazorPages();

app.Run();
