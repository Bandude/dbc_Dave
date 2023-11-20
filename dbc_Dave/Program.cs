
using dbc_Dave.Components.Account;
using Microsoft.EntityFrameworkCore;
using dbc_Dave.Services;
using dbc_Dave.Data;
using Microsoft.AspNetCore.Identity.UI.Services;
using Microsoft.AspNetCore.HttpOverrides;
using dbc_Dave.Services.Interfaces;
using Microsoft.IdentityModel.Tokens;
using System.Text;
using Microsoft.AspNetCore.Components.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Components;
using dbc_Dave.Components;
using dbc_Dave.Controllers;



public class Program
{
    public static void Main(string[] args)
    {


        // Initiate and build configuration
        var builder = WebApplication.CreateBuilder(args);
        var environment = builder.Environment;
        var apiKey = builder.Configuration.GetValue<string>("OpenAi:ApiKey") ?? Environment.GetEnvironmentVariable("OpenAi_ApiKey") ?? "";
        var redisHost = builder.Configuration.GetValue<string>("RedisHost") ?? "localhost:6379";
        // Add environment-specific configuration sources
        if (environment.IsDevelopment())
        {
            builder.Configuration.AddJsonFile("appsettings.Development.json", optional: true);
        }
        // Add environment variables configuration source
        builder.Configuration.AddEnvironmentVariables();

        builder.Services.AddAntiforgery(options => { options.Cookie.Expiration = TimeSpan.Zero; });


        // Add services to the container.
        builder.Services.AddRazorComponents()
            .AddInteractiveServerComponents();

        builder.Services.AddAntiforgery();

        builder.Services.AddCascadingAuthenticationState();
        builder.Services.AddScoped<IdentityUserAccessor>();
        builder.Services.AddScoped<IdentityRedirectManager>();
        builder.Services.AddScoped<AuthenticationStateProvider, IdentityRevalidatingAuthenticationStateProvider>();



        var connectionString = builder.Configuration.GetConnectionString("usersContextConnection") ?? throw new InvalidOperationException("Connection string 'usersContextConnection' not found.");
        builder.Services.AddDbContextFactory<ApplicationDbContext>(options =>
            options.UseSqlServer(connectionString));



        builder.Services.AddIdentityCore<ApplicationUser>(options => options.SignIn.RequireConfirmedAccount = true)
            .AddEntityFrameworkStores<ApplicationDbContext>()
            .AddSignInManager()
            .AddDefaultTokenProviders();

        builder.Services.AddIdentityApiEndpoints<ApplicationUser>()
            .AddEntityFrameworkStores<ApplicationDbContext>();

        builder.Services.AddSingleton<IEmailSender<ApplicationUser>, IdentityNoOpEmailSender>();


        builder.Services.AddServerSideBlazor();

        builder.Services.AddSwaggerGen();

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
                redisHost,
                provider.GetRequiredService<IDbContextFactory<ApplicationDbContext>>(),
                provider.GetRequiredService<ILogger<RedisService>>()
            )
        );
        builder.Services.AddSingleton<IAssistantService>(provider =>
            new AssistantService(
            apiKey,
            provider.GetRequiredService<ILogger<AssistantService>>()
            )
        );
        builder.Services.AddSingleton<IOpenAI>(provider =>
            new OpenAI(
                apiKey,
                provider.GetRequiredService<ILogger<OpenAI>>()
            )
        );
        builder.Services.AddSingleton<IThreadService>(provider =>
            new ThreadService(
                apiKey,
                provider.GetRequiredService<ILogger<ThreadService>>()
            )
        );
        builder.Services.AddSingleton<IMessageService>(provider =>
            new MessageService(
                apiKey,
                provider.GetRequiredService<ILogger<MessageService>>()
            )
        );

        builder.Services.AddControllers();

        // Add singleton services
        builder.Services.AddScoped<IOpenAI>(provider => new OpenAI(apiKey, provider.GetRequiredService<ILogger<OpenAI>>()));
        builder.Services.AddSingleton<IThreadService>(provider => new ThreadService(apiKey, provider.GetRequiredService<ILogger<ThreadService>>()));
        builder.Services.AddSingleton<IMessageService>(provider => new MessageService(apiKey, provider.GetRequiredService<ILogger<MessageService>>()));
        builder.Services.AddScoped(provider => new APIController(provider.GetService<IOpenAI>(), provider.GetService<IRedisService>(), provider.GetService<ILogger<APIController>>(), provider.GetRequiredService<IServiceProvider>()));

        // Add logging 
        builder.Services.AddLogging(configure => configure
                    .AddConsole() // Use the console logger.
                    .SetMinimumLevel(Microsoft.Extensions.Logging.LogLevel.Error) // Set the minimum log level.
                );

        // Add services to the container.
        builder.Services.AddRazorComponents()
            .AddInteractiveServerComponents();
        builder.Services
            .AddServerSideBlazor()
            .AddHubOptions(x => x.MaximumReceiveMessageSize = 102400000);
        builder.Services.AddHttpContextAccessor();
        builder.Services.AddHttpClient();
        builder.Services.AddScoped<dbc_Dave.Services.Utility>();


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


        // Use HTTPS redirection
        app.UseHttpsRedirection();

        // Use forwarded headers
        app.UseForwardedHeaders();

        // Use static files
        app.UseStaticFiles();

        // Use routing
        app.UseRouting();

        // Use authentication
        //app.UseAuthentication();

        // Use authorization
        app.UseAuthorization();

        // Add the anti-forgery middleware at this point
        app.UseAntiforgery();

        // Configure the endpoints
        app.MapControllers();

        // Set up endpoints
        app.MapRazorComponents<App>()
                .AddInteractiveServerRenderMode();


        app.MapAdditionalIdentityEndpoints();

        app.MapIdentityApi<ApplicationUser>();

        if (environment.IsDevelopment())
        {
            app.UseSwagger();
            app.UseSwaggerUI();
        }
       
        // Run the application
        app.Run();

  
    }

}

