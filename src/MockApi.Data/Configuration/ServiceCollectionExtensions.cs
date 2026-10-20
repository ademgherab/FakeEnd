using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace MockApi.Data.Configuration;

public static class ServiceCollectionExtensions
{
    /// <summary>
    /// Binds and validates the PostgreSQL connection string at startup. The DbContext itself is
    /// wired in T01.
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
}
