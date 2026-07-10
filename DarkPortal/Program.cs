using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Identity;
using MudBlazor.Services;
using DarkPortal.Components;
using DarkPortal.Data;
using DarkPortal.Models;
using DarkPortal.Services;

var builder = WebApplication.CreateBuilder(args);

// База данных SQLite
builder.Services.AddDbContext<AppDbContext>(options =>
    options.UseSqlite("Data Source=darkportal.db"));

// Identity
builder.Services.AddIdentity<AppUser, IdentityRole>(options =>
{
    options.Password.RequireDigit = false;
    options.Password.RequiredLength = 4;
    options.Password.RequireNonAlphanumeric = false;
    options.Password.RequireUppercase = false;
    options.Password.RequireLowercase = false;
})
.AddEntityFrameworkStores<AppDbContext>()
.AddRoles<IdentityRole>()
.AddDefaultTokenProviders();

builder.Services.ConfigureApplicationCookie(options =>
{
    options.LoginPath = "/auth";
    options.AccessDeniedPath = "/";
});

builder.Services.AddControllers();
builder.Services.AddRazorComponents().AddInteractiveServerComponents();
builder.Services.AddMudServices();
builder.Services.AddHttpClient();

// Сервисы
builder.Services.AddScoped<LocalStorageService>();
builder.Services.AddScoped<ProfileService>();
builder.Services.AddScoped<AuthService>();
builder.Services.AddScoped<NewsService>();
builder.Services.AddScoped<LessonService>();
builder.Services.AddScoped<AdminService>();
builder.Services.AddScoped<AchievementService>();
builder.Services.AddSingleton<CryptoService>();
builder.Services.AddSingleton<LanguageService>();
builder.Services.AddSingleton<LocaleService>();
builder.Services.AddSingleton<NotificationService>();
builder.Services.AddScoped<MessageService>();
builder.Services.AddHostedService<MessageCleanupService>();
builder.Services.AddScoped<ForumService>();
builder.Services.AddScoped<AntiSpamService>();
builder.Services.AddScoped<FriendService>();

builder.Services.AddHttpClient<TranslationService>();
builder.Services.AddHttpClient<HibpService>();
builder.Services.AddHostedService<RssBackgroundService>();

builder.Services.AddHttpContextAccessor();

var app = builder.Build();

// Инициализация БД и ролей
using (var scope = app.Services.CreateScope())
{
    var db = scope.ServiceProvider.GetRequiredService<AppDbContext>();
    db.Database.EnsureCreated();

    var roleManager = scope.ServiceProvider.GetRequiredService<RoleManager<IdentityRole>>();
    if (!await roleManager.RoleExistsAsync("User"))
        await roleManager.CreateAsync(new IdentityRole("User"));
    if (!await roleManager.RoleExistsAsync("Admin"))
        await roleManager.CreateAsync(new IdentityRole("Admin"));
}

if (!app.Environment.IsDevelopment()) { app.UseExceptionHandler("/Error"); app.UseHsts(); }

app.UseStatusCodePagesWithReExecute("/not-found");
app.UseHttpsRedirection();
app.UseAntiforgery();
app.UseAuthentication();
app.UseAuthorization();
app.MapStaticAssets();

// ========== ЛОГИН ==========
app.MapPost("/api/login", async (HttpContext context) =>
{
    var form = await context.Request.ReadFormAsync();
    var username = form["username"].ToString();
    var password = form["password"].ToString();

    var userManager = context.RequestServices.GetRequiredService<UserManager<AppUser>>();
    var db = context.RequestServices.GetRequiredService<AppDbContext>();
    var user = await userManager.FindByNameAsync(username);

    if (user != null && await userManager.IsLockedOutAsync(user))
    {
        var ban = await db.BanRecords
            .Where(b => b.UserId == user.Id && b.UnbannedAt == null)
            .OrderByDescending(b => b.BannedAt)
            .FirstOrDefaultAsync();

        var reason = ban?.Reason ?? "Не указана";
        var adminName = ban?.AdminName ?? "Неизвестно";
        var adminId = ban?.AdminId ?? "";
        var localTime = (ban?.BannedAt ?? DateTime.UtcNow).AddHours(3);
        var date = localTime.ToString("yyyy-MM-dd HH:mm");

        context.Response.Redirect($"/banned?reason={Uri.EscapeDataString(reason)}&admin={Uri.EscapeDataString(adminName)}&date={Uri.EscapeDataString(date)}&adminId={Uri.EscapeDataString(adminId)}&userName={Uri.EscapeDataString(username)}&userId={Uri.EscapeDataString(user.Id)}");
        return;
    }

    var signInManager = context.RequestServices.GetRequiredService<SignInManager<AppUser>>();
    var result = await signInManager.PasswordSignInAsync(username, password, true, false);
    context.Response.Redirect(result.Succeeded ? "/" : "/auth?error=invalid");
}).DisableAntiforgery();

// ========== РЕГИСТРАЦИЯ ==========
app.MapPost("/api/register", async (HttpContext context) =>
{
    var form = await context.Request.ReadFormAsync();
    var username = form["username"].ToString().Trim();
    var password = form["password"].ToString();
    var captchaAnswer = form["captcha"].ToString();
    var captchaExpected = form["captchaExpected"].ToString();

    if (captchaAnswer != captchaExpected) { context.Response.Redirect("/auth?error=captcha"); return; }

    var ip = context.Connection.RemoteIpAddress?.ToString() ?? "unknown";
    var userManager = context.RequestServices.GetRequiredService<UserManager<AppUser>>();
    var signInManager = context.RequestServices.GetRequiredService<SignInManager<AppUser>>();
    var authService = context.RequestServices.GetRequiredService<AuthService>();
    var (success, error) = await authService.Register(username, password, ip);

    if (success)
    {
        var recoveryCode = Guid.NewGuid().ToString("N")[..12].ToUpper();
        var user = await userManager.FindByNameAsync(username);
        if (user != null) { user.RecoveryCode = recoveryCode; await userManager.UpdateAsync(user); }
        await signInManager.SignInAsync(user!, true);
        context.Response.Redirect($"/auth?registered=ok&code={recoveryCode}");
        return;
    }
    context.Response.Redirect($"/auth?error=reglimit&msg={Uri.EscapeDataString(error)}");
}).DisableAntiforgery();

// ========== ВОССТАНОВЛЕНИЕ ПАРОЛЯ ==========
app.MapPost("/api/reset-password", async (HttpContext context) =>
{
    var form = await context.Request.ReadFormAsync();
    var userManager = context.RequestServices.GetRequiredService<UserManager<AppUser>>();
    var user = await userManager.FindByNameAsync(form["username"].ToString());
    if (user == null || user.RecoveryCode != form["recoverycode"].ToString().Trim().ToUpper()) { context.Response.Redirect("/auth?error=badcode"); return; }
    var token = await userManager.GeneratePasswordResetTokenAsync(user);
    var result = await userManager.ResetPasswordAsync(user, token, form["newpassword"].ToString());
    context.Response.Redirect(result.Succeeded ? "/auth?reset=ok" : "/auth?error=reseterror");
}).DisableAntiforgery();

// ========== ЛОГАУТ ==========
app.MapGet("/api/logout", async (HttpContext context) =>
{
    await context.RequestServices.GetRequiredService<SignInManager<AppUser>>().SignOutAsync();
    context.Response.Redirect("/");
});

app.MapControllers();
app.MapRazorComponents<App>().AddInteractiveServerRenderMode();

app.Run();