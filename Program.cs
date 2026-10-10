
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.EntityFrameworkCore;
using Rotativa.AspNetCore;
using TMSWeb.Models.Entities.HR;
using TMSWeb.Models.Entities.LOG;
using TMSWeb.Repositories;
using TMSWeb.Services;

var builder = WebApplication.CreateBuilder(args);

// =====================================================
// MVC
// =====================================================

builder.Services.AddControllersWithViews();
builder.Services.AddHttpContextAccessor();

builder.Services.AddSingleton<
    IDispatchService,
    DemoDispatchService>();

// =====================================================
// Database
// =====================================================

builder.Services.AddDbContext<DbavalogContext>(options =>
{
    options.UseSqlServer(
        builder.Configuration.GetConnectionString("servLOG"));
});

builder.Services.AddDbContext<DbavahrContext>(options =>
{
    options.UseSqlServer(
        builder.Configuration.GetConnectionString("servHR"));
});

// =====================================================
// Cookie Authentication (ไม่ใช้ SSO)
// =====================================================

builder.Services
    .AddAuthentication(
        CookieAuthenticationDefaults.AuthenticationScheme)
    .AddCookie(options =>
    {
        // Cookie Name
        options.Cookie.Name = "TMSWeb.Auth";

        // Cookie Security
        options.Cookie.HttpOnly = true;
        options.Cookie.IsEssential = true;
        options.Cookie.SecurePolicy =
            CookieSecurePolicy.Always;

        options.Cookie.SameSite = SameSiteMode.Lax;

        // Login / Logout / Access Denied
        options.LoginPath = "/Account/Login";
        options.LogoutPath = "/Account/Logout";
        options.AccessDeniedPath = "/Account/AccessDenied";

        // Cookie Expiration
        options.ExpireTimeSpan = TimeSpan.FromHours(8);

        // ต่ออายุ Cookie เมื่อใช้งาน
        options.SlidingExpiration = true;

        // ReturnUrl
        options.ReturnUrlParameter = "ReturnUrl";
    });

// =====================================================
// Authorization
// =====================================================

builder.Services.AddAuthorization();

// =====================================================
// Repository / Services
// =====================================================

builder.Services
    .AddApplicationRepositories()
    .AddApplicationServices();

builder.Services.AddScoped<AttendanceExcelService>();

// =====================================================
// Build
// =====================================================

var app = builder.Build();

// =====================================================
// Error Handling
// =====================================================

if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Home/Error");
    app.UseHsts();
}

// =====================================================
// HTTPS
// =====================================================

app.UseHttpsRedirection();

// =====================================================
// Static Files
// =====================================================

app.UseStaticFiles();

// =====================================================
// Routing
// =====================================================

app.UseRouting();

// =====================================================
// Authentication
// =====================================================

app.UseAuthentication();

// =====================================================
// Authorization
// =====================================================

app.UseAuthorization();

// =====================================================
// MVC Route
// =====================================================

app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=Index}/{id?}");

// =====================================================
// Rotativa
// =====================================================

RotativaConfiguration.Setup(
    app.Environment.ContentRootPath,
    "wwwroot/Rotativa");

// =====================================================
// Run
// =====================================================

app.Run();
