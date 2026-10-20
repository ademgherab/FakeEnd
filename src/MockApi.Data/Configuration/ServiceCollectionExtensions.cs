using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using MockApi.Data.Repositories;

namespace MockApi.Data.Configuration;

public static class ServiceCollectionExtensions
{
    /// <summary>
    /// Binds and validates the PostgreSQL connection string at startup.
    /// </summary>
    public static IServiceCollection AddMockApiDatabaseOptions(
        this IServiceCollection services,
        IConfiguration configuration
    )
    {
        ArgumentNullException.ThrowIfNull(services);
        ArgumentNullException.ThrowIfNull(configuration);

        services
            .AddOptions<DatabaseOptions>()
            .Bind(configuration.GetSection(DatabaseOptions.SectionName))
            .ValidateDataAnnotations()
            .ValidateOnStart();

        return services;
    }

    public static IServiceCollection AddMockApiData(
        this IServiceCollection services,
        IConfiguration configuration
    )
    {
        ArgumentNullException.ThrowIfNull(services);
        ArgumentNullException.ThrowIfNull(configuration);

        var connectionString = configuration.GetConnectionString("Default");

        if (string.IsNullOrWhiteSpace(connectionString))
        {
            throw new InvalidOperationException(
                "ConnectionStrings:Default must contain a PostgreSQL connection string."
            );
        }

        services.AddDbContext<MockApiDbContext>(options =>
            options
                .UseNpgsql(
                    connectionString,
                    npgsql =>
                        npgsql.MigrationsAssembly(typeof(MockApiDbContext).Assembly.GetName().Name!)
                )
                .UseSnakeCaseNamingConvention()
        );

        services.AddScoped<IProjectRepository, ProjectRepository>();
        services.AddScoped<IRequestLogRepository, RequestLogRepository>();
        services.AddScoped<IUsageRepository, UsageRepository>();

        return services;
    }
}
