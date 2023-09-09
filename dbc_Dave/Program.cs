
using dbc_Dave.Areas.Identity.Data;
using Microsoft.EntityFrameworkCore;
using dbc_Dave.Services;
using dbc_Dave.Data;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.UI.Services;
using dbc_Dave.Data.Models;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Identity.Client;
using Microsoft.AspNetCore.HttpOverrides;
using System.Net;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.OAuth;

public class Program
{
    public static void Main(string[] args)
    {


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
        var redishost = builder.Configuration.GetValue<string>("RedisHost") ?? "localhost:6379";

        // Set the connection string
        var connectionString = builder.Configuration.GetConnectionString("usersContextConnection") ?? throw new InvalidOperationException("Connection string 'usersContextConnection' not found.");

        builder.Services.AddDbContext<dbc_UsersContext>(options =>
            options.UseSqlServer(connectionString));

        builder.Services.Configure<ForwardedHeadersOptions>(options =>
        {
            options.ForwardedHeaders =
                ForwardedHeaders.XForwardedFor | ForwardedHeaders.XForwardedProto | ForwardedHeaders.XForwardedHost;
        });

        builder.Services.AddDefaultIdentity<User>()
            .AddEntityFrameworkStores<dbc_UsersContext>();



        builder.Services.AddAuthorization();
        builder.Services.AddAuthentication();
        //builder.Services.AddAuthentication()
        //    .AddGoogle(options =>
        //    {
        //        IConfigurationSection googleAuthNSection = builder.Configuration.GetSection("Authentication:Google");
        //        options.ClientId = googleAuthNSection["ClientId"] ?? "";
        //        options.ClientSecret = googleAuthNSection["ClientSecret"] ?? "";
        //        options.CallbackPath = new PathString("/ExternalLogin");

        //    });


        if (environment.IsProduction())
        {
            builder.Services.AddHsts(options =>
            {
                options.Preload = true;
                options.IncludeSubDomains = true;
                options.MaxAge = TimeSpan.FromDays(60);
            });
        }

        builder.Services.AddScoped<IRedisService>(provider =>
            new RedisService(
                redishost,
                provider.GetRequiredService<dbc_UsersContext>(),
                provider.GetRequiredService<ILogger<RedisService>>() // Use ILogger<RedisService> or the appropriate ILogger type
            )
        );

        // Add singleton services
        builder.Services.AddSingleton<IOpenAI>(provider => new OpenAI(apiKey, provider.GetRequiredService<ILogger<OpenAI>>()));
        builder.Services.AddSingleton<IEmailSender, DummyEmailSender>();
        // Add logging 
        builder.Services.AddLogging(configure => configure
                    .AddConsole() // Use the console logger.
                    .SetMinimumLevel(Microsoft.Extensions.Logging.LogLevel.Error) // Set the minimum log level.
                );

        // Add services to the container.
        builder.Services.AddRazorPages();
        builder.Services
            .AddServerSideBlazor()
            .AddHubOptions(x => x.MaximumReceiveMessageSize = 102400000);
        builder.Services.AddHttpContextAccessor();
        builder.Services.AddHttpClient();
        builder.Services.AddScoped<Utility>();


        builder.Services.Configure<CookiePolicyOptions>(options =>
        {
            options.CheckConsentNeeded = context => true;
            options.MinimumSameSitePolicy = SameSiteMode.None;
            options.Secure = CookieSecurePolicy.Always;
        });
        // Build the application
        var app = builder.Build();



        app.UseCookiePolicy();


        if (environment.IsProduction())
        {
            app.UseHsts();
        }
        app.UseHttpsRedirection();

        app.UseForwardedHeaders();


        app.UseStaticFiles();


        app.UseRouting();

        // Set up endpoints
        app.MapBlazorHub();
        app.MapFallbackToPage("/_Host");

        app.UseAuthentication();
        app.UseAuthorization();


        // Run the application
        app.Run();
    }

}

