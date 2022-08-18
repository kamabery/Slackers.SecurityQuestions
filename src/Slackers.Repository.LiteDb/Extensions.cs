using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace Slackers.Repository.LiteDb;

public static class Extensions
{
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