
using dbc_Dave.Areas.Identity.Data;
using Microsoft.EntityFrameworkCore;
using dbc_Dave.Services;
using dbc_Dave.Data;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.UI.Services;
using dbc_Dave.Data.Models;

// Initiate and build configuration
var builder = WebApplication.CreateBuilder(args);
var environment = builder.Environment;

// Add environment-specific configuration sources
if (environment.IsDevelopment())
{
    builder.Configuration.AddJsonFile("appsettings.Development.json", optional: true);
}

// Add environment variables configuration source
builder.Configuration.AddEnvironmentVariables();

var apiKey = builder.Configuration.GetValue<string>("OpenAi:ApiKey") ?? Environment.GetEnvironmentVariable("OpenAi_ApiKey") ?? "";

// Set the connection string
var connectionString = builder.Configuration.GetConnectionString("usersContextConnection") ?? throw new InvalidOperationException("Connection string 'personalAssistantContextConnection' not found.");

builder.Services.AddDbContext<dbc_UsersContext>(options =>
    options.UseSqlServer(connectionString));


builder.Services.AddDefaultIdentity<User>()
    .AddEntityFrameworkStores<dbc_UsersContext>();



builder.Services.AddAuthorization();
builder.Services.AddAuthentication()
    .AddGoogle(options =>
    {
        IConfigurationSection googleAuthNSection = builder.Configuration.GetSection("Authentication:Google");
        options.ClientId = googleAuthNSection["ClientId"] ?? "";
        options.ClientSecret = googleAuthNSection["ClientSecret"] ?? "";
        options.CallbackPath = new PathString("/ExternalLogin");
    });

builder.Services.AddScoped<IRedisService, RedisService>();

// Add singleton services
builder.Services.AddSingleton<IOpenAI>(provider => new OpenAI(apiKey, provider.GetRequiredService<ILogger<OpenAI>>()));
builder.Services.AddSingleton<IEmailSender, DummyEmailSender>();
// Add logging 
builder.Services.AddLogging(configure => configure
            .AddConsole() // Use the console logger.
            .SetMinimumLevel(LogLevel.Error) // Set the minimum log level.
        );

// Add services to the container.
builder.Services.AddRazorPages();
builder.Services
    .AddServerSideBlazor()
    .AddHubOptions(x => x.MaximumReceiveMessageSize = 102400000);
builder.Services.AddHttpContextAccessor();
builder.Services.AddScoped<Utility>();

// Build the application
var app = builder.Build();

// Configure the HTTP request pipeline.
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Error");
    app.UseHsts(); // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
}

app.UseHttpsRedirection();
app.UseStaticFiles();
app.UseRouting();

// Set up endpoints
app.MapBlazorHub();
app.MapFallbackToPage("/_Host");
app.UseAuthentication();
app.UseAuthorization();

// Run the application
app.Run();