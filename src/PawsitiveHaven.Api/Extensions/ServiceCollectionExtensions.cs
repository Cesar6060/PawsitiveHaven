using System.Text;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using PawsitiveHaven.Api.Data;
using PawsitiveHaven.Api.Data.Repositories;
using PawsitiveHaven.Api.Services;

namespace PawsitiveHaven.Api.Extensions;

public static class ServiceCollectionExtensions
{
    public static IServiceCollection AddDatabase(this IServiceCollection services, IConfiguration configuration)
    {
        var connectionString = configuration.GetConnectionString("DefaultConnection")
            ?? Environment.GetEnvironmentVariable("DATABASE_URL")
            ?? "Host=localhost;Database=pawsitive_haven;Username=pawsitive;Password=pawsitive123";

        services.AddDbContext<AppDbContext>(options =>
            options.UseNpgsql(connectionString));

        return services;
    }

    public static IServiceCollection AddRepositories(this IServiceCollection services)
    {
        services.AddScoped(typeof(IRepository<>), typeof(Repository<>));
        services.AddScoped<IUserRepository, UserRepository>();
        services.AddScoped<IPetRepository, PetRepository>();
        services.AddScoped<IAppointmentRepository, AppointmentRepository>();
        services.AddScoped<IFaqRepository, FaqRepository>();
        services.AddScoped<IConversationRepository, ConversationRepository>();

        return services;
    }

    public static IServiceCollection AddApplicationServices(this IServiceCollection services)
    {
        // Core services
        services.AddScoped<IPasswordHasher, PasswordHasher>();
        services.AddScoped<IJwtService, JwtService>();
        services.AddScoped<IAuthService, AuthService>();
        services.AddScoped<IPetService, PetService>();
        services.AddScoped<IAppointmentService, AppointmentService>();
        services.AddScoped<IFaqService, FaqService>();

        // AI and security services
        services.AddMemoryCache();
        services.AddSingleton<IChatSecurityService, ChatSecurityService>();
        services.AddSingleton<IRateLimitService, RateLimitService>();
        services.AddScoped<IAiService, AiService>();

        return services;
    }

    public static IServiceCollection AddJwtAuthentication(this IServiceCollection services, IConfiguration configuration)
    {
        var jwtSecret = configuration["Jwt:SecretKey"]
            ?? Environment.GetEnvironmentVariable("JWT_SECRET")
            ?? "DefaultSecretKeyThatShouldBeAtLeast32Characters!";

        var key = Encoding.UTF8.GetBytes(jwtSecret);

        services.AddAuthentication(options =>
        {
            options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
            options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
        })
        .AddJwtBearer(options =>
        {
            options.RequireHttpsMetadata = false;
            options.SaveToken = true;
            options.TokenValidationParameters = new TokenValidationParameters
            {
                ValidateIssuerSigningKey = true,
                IssuerSigningKey = new SymmetricSecurityKey(key),
                ValidateIssuer = false,
                ValidateAudience = false,
                ClockSkew = TimeSpan.Zero
            };
        });

        services.AddAuthorization(options =>
        {
            options.AddPolicy("AdminOnly", policy => policy.RequireRole("Admin"));
        });

        return services;
    }

    public static IServiceCollection AddCorsPolicy(this IServiceCollection services, IConfiguration configuration)
    {
        services.AddCors(options =>
        {
            options.AddPolicy("AllowFrontend", policy =>
            {
                policy.WithOrigins(
                        "http://localhost:5180",
                        "http://localhost:5181",
                        "https://localhost:5180",
                        "https://localhost:5181"
                    )
                    .AllowAnyMethod()
                    .AllowAnyHeader()
                    .AllowCredentials();
            });
        });

        return services;
    }
}
