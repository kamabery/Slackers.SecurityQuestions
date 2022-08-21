using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace Slackers.Repository.LiteDb;

/// <summary>
/// Helper class to extend a service collection to add a repository
/// </summary>
public static class LiteDbExtensions
{
    /// <summary>
    /// A Helper method to extend a service collection to add repository to DI
    /// </summary>
    /// <param name="services"></param>
    /// <param name="configuration"></param>
    /// <param name="section"></param>
    /// <returns></returns>
    public static IServiceCollection AddRepository(
        this IServiceCollection services,
        IConfiguration configuration,
        string section = "SqlLite")
    {
        services.Configure<SqlLiteRepositoryOptions>(configuration.GetSection(section));
        services.AddTransient<IRepository, LiteDbRepository>();
        return services;
    }

}