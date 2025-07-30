
using E_Commers_Adelia.Data;
using Microsoft.EntityFrameworkCore;
using Serilog;
using Microsoft.AspNetCore.Identity;
using E_Commers_Adelia.Models;
using E_Commers_Adelia.Service;
using E_Commers_Adelia.Common;
using Microsoft.AspNetCore.SignalR;
using E_Commers_Adelia.Hub;

DotNetEnv.Env.Load(); // ← baca .env file

var builder = WebApplication.CreateBuilder(args);

// Setup Serilog
Log.Logger = new LoggerConfiguration()
    .MinimumLevel.Information()
    .WriteTo.Console()
    .WriteTo.File("Logs/app-.log", rollingInterval: RollingInterval.Day)
    .Enrich.FromLogContext()
    .CreateLogger();

builder.Host.UseSerilog(); // Guna Serilog sebagai logger host

// Db context configuration
string? dbHost = EnvHelper.GetEnv("DB_HOST");
string? dbPort = EnvHelper.GetEnv("DB_PORT");
string? dbName = EnvHelper.GetEnv("DB_NAME");
string? dbUser = EnvHelper.GetEnv("DB_USER");
string? dbPassword = EnvHelper.GetEnv("DB_PASS");
string connectionString = $"host={dbHost};port={dbPort};Database={dbName};username={dbUser};Password={dbPassword};";

try
{

    builder.Services.AddDbContext<ApplicationDbContext>(options =>
        options.UseNpgsql(connectionString));
}
catch (Exception ex)
{
    Log.Fatal(ex, "Failed to connect to the database. Please check your connection string.");
    throw;
}

builder.Services.AddDefaultIdentity<EUser>(options => options.SignIn.RequireConfirmedAccount = true)
    .AddRoles<IdentityRole>()
    .AddEntityFrameworkStores<ApplicationDbContext>();

// Add Id provider for SignalR
builder.Services.AddSingleton<IUserIdProvider, ProviderId>();

// active session
builder.Services.AddSession();

// Add services to the container.
builder.Services.AddControllersWithViews().AddRazorRuntimeCompilation();

// Add SignalR
builder.Services.AddSignalR();

// add MailService
builder.Services.AddTransient<IEmailService, SMTPEmailSender>();

var app = builder.Build();

app.UseSession();

// Configure the HTTP request pipeline.
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Home/Error");
    // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
    app.UseHsts();

   
}

    using (var scope = app.Services.CreateScope())
{
    var services = scope.ServiceProvider;

    var context = services.GetRequiredService<ApplicationDbContext>();
    if (context.Database.GetPendingMigrations().Any())
    {
        context.Database.Migrate();
    }
}
using (var scope = app.Services.CreateScope())
{
    var roleManager = scope.ServiceProvider.GetRequiredService<RoleManager<IdentityRole>>();
    var roles = new[] { "Admin", "Seller","Customer","Guest" };

    foreach (var role in roles)
    {
        if (!await roleManager.RoleExistsAsync(role))
            await roleManager.CreateAsync(new IdentityRole(role));
    }
}

using (var scope = app.Services.CreateScope())
{
    var userManager = scope.ServiceProvider.GetRequiredService<UserManager<EUser>>();
    var _logger = scope.ServiceProvider.GetRequiredService<ILogger<EUser>>();


    string email = "admin@example.com";
    string password = "P@ssw0rd";

    if (await userManager.FindByEmailAsync(email) == null)
    {
        var user = new EUser
        {
            Email = email,
            StoreName = "Kedai Admin",
            UserName = email,
            DisplayName = "SystemAdmin",
            EmailConfirmed = true,
            IsActive = true,
            CreatedAt = DateTime.UtcNow
        };

        var result = await userManager.CreateAsync(user, password);
        if (result.Succeeded)
        {
            await userManager.AddToRoleAsync(user, "Admin");
        }
        else
        {
            foreach (var error in result.Errors)
            {
                Log.Error(error.Description, DateTime.Today.ToLongDateString());
            }
        }
    }
}

app.UseHttpsRedirection();
app.UseRouting();
app.UseStaticFiles();
app.UseAuthentication();
app.UseAuthorization();
app.MapStaticAssets();

app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=Index}/{id?}")
    .WithStaticAssets();

app.MapRazorPages(); // ✅ WAJIB untuk Identity Razor Pages

app.MapHub<NotificationHub>("/notificationHub"); // Map SignalR hub

app.Run();
