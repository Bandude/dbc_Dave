
using dbc_Dave.Areas.Identity.Data;
using Microsoft.EntityFrameworkCore;
using dbc_Dave.Services;
using dbc_Dave.Data;
using Microsoft.AspNetCore.Identity.UI.Services;
using Microsoft.AspNetCore.HttpOverrides;
using dbc_Dave.Services.Interfaces;
using Microsoft.IdentityModel.Tokens;
using System.Text;

using Microsoft.AspNetCore.Identity;

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
        builder.Services.AddDbContextFactory<dbc_UsersContext>(options => options.UseSqlServer(connectionString));

        builder.Services.Configure<ForwardedHeadersOptions>(options =>
        {
            options.ForwardedHeaders =
                ForwardedHeaders.XForwardedFor | ForwardedHeaders.XForwardedProto | ForwardedHeaders.XForwardedHost;
        });

        builder.Services.AddDefaultIdentity<User>()
            .AddEntityFrameworkStores<dbc_UsersContext>();



        var jwtSettings = builder.Configuration.GetSection("JwtSettings");
        builder.Services.AddAuthorization();
        //builder.Services.AddAuthentication(options =>
        //{
        //    // This is the Default scheme that's used when no scheme is specified.
        //    // For example, [Authorize] without a scheme will use this by default.
        //    options.DefaultScheme = IdentityConstants.ExternalScheme;
        //    options.DefaultSignInScheme = IdentityConstants.ExternalScheme;

        //    // This is the scheme for authenticating users with JWT bearer tokens
        //    options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
        //    options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
        //})
        //        .AddJwtBearer(options =>
        //{
        //    options.TokenValidationParameters = new TokenValidationParameters
        //    {
        //        ValidateIssuer = true,
        //        ValidateAudience = true,
        //        ValidateLifetime = true,
        //        ValidateIssuerSigningKey = true,
        //        ValidIssuer = jwtSettings["Issuer"],
        //        ValidAudience = jwtSettings["Audience"],
        //        IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(jwtSettings["Key"]))
        //    };
        //});
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


        builder.Services.AddDbContextFactory<dbc_UsersContext>();
        builder.Services.AddScoped<IRedisService>(provider =>
            new RedisService(
                redishost,
                provider.GetRequiredService<IDbContextFactory<dbc_UsersContext>>(),
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
        builder.Services.AddSingleton<IOpenAI>(provider => new OpenAI(apiKey, provider.GetRequiredService<ILogger<OpenAI>>()));
        builder.Services.AddSingleton<ThreadService>(provider => new ThreadService(apiKey, provider.GetRequiredService<ILogger<ThreadService>>()));
        builder.Services.AddSingleton<MessageService>(provider => new MessageService(apiKey, provider.GetRequiredService<ILogger<MessageService>>()));

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
        app.UseHttpsRedirection();
        app.UseForwardedHeaders();
        app.UseStaticFiles();
        //app.MapControllers();
        app.UseRouting();


        // Set up endpoints
        app.MapBlazorHub();
        app.MapControllers();
        app.MapFallbackToPage("/_Host");
        app.UseAuthentication();
        app.UseAuthorization();

        // Run the application
        app.Run();
    }

}

