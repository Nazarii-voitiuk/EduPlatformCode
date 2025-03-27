using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using EduCodePlatform.Data;
using EduCodePlatform.Models.Identity; // ��� ���� ApplicationUser

var builder = WebApplication.CreateBuilder(args);

// ϳ��������� ��
builder.Services.AddDbContext<ApplicationDbContext>(options =>
    options.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection")));

// Identity � ����� ApplicationUser : IdentityUser
builder.Services.AddDefaultIdentity<ApplicationUser>(options =>
{
    options.Password.RequireDigit = false;
    options.Password.RequireNonAlphanumeric = false;
    options.Password.RequiredLength = 6;
})
.AddEntityFrameworkStores<ApplicationDbContext>();

// �������, ���� ������� ��������������� ������� Razor Area Identity
builder.Services.AddRazorPages();

// ����� ����� .AddControllersWithViews()
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

// ��������
app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=Index}/{id?}"
);

// ��� Razor Pages Identity
app.MapRazorPages();

app.Run();
