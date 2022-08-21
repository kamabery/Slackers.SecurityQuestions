using Microsoft.Extensions.Configuration;
using Serilog.Formatting.Json;
using Serilog;
using System.Diagnostics;
using System.Reflection;
using Microsoft.Extensions.Hosting;
using Serilog.Enrichers.Span;

namespace Slackers.Logging;

/// <summary>
/// Helper class to extend HostBuilder to add Structured.
/// </summary>
public static class LoggingSetupExtension
{
    // <summary>
    /// Helper method to extend IHostBuilder to ddd structured logging.
    /// <summary>
    /// </summary>
    /// <param name="builder"><see cref="IHostBuilder"/></param>
    /// <param name="loggingSection">Section in which Logging Configuration</param>
    public static void AddLogging(this IHostBuilder builder, string loggingSection)
    {
        var name = string.Empty;
        var s = Assembly.GetCallingAssembly().GetName().Name;
        if (s != null) name = s.ToLower();

        builder.UseSerilog((ctx, lc) =>
        {
            SetupLoggingConfiguration(lc, ctx.Configuration, loggingSection, name);
        });
    }

    private static void SetupLoggingConfiguration(
            LoggerConfiguration loggingConfiguration,
            IConfiguration configuration,
            string loggingSection,
            string applicationName)
    {
        var options = configuration.GetSection(loggingSection).Get<LoggingOptions>();
        if (options == null)
        {
            options = new LoggingOptions
            {
                FileLocation = Environment.CurrentDirectory,
                RollingInterval = 6
            };
        }

        if (string.IsNullOrEmpty(options.FileName))
        {
            options.FileName = applicationName;
        }

        if (string.IsNullOrEmpty(options.FileLocation))
        {
            options.FileLocation = Environment.CurrentDirectory;
        }

        // For Tracing
        Activity.DefaultIdFormat = ActivityIdFormat.W3C;


        var environment = Environment.GetEnvironmentVariable("ASPNETCORE_ENVIRONMENT");


        loggingConfiguration.Enrich.FromLogContext().Enrich
            .WithProperty("Environment", environment)
            //ILogger Begin Scope;
            .Enrich.FromLogContext().Enrich.WithSpan();

        if (!string.IsNullOrEmpty(options.FileLocation))
        {
            var interval = RollingInterval.Day;
            switch (options.RollingInterval)
            {
                case 1:
                    interval = RollingInterval.Minute;
                    break;
                case 2:
                    interval = RollingInterval.Hour;
                    break;
                case 3:
                    interval = RollingInterval.Month;
                    break;
                case 4:
                    interval = RollingInterval.Year;
                    break;
                case 5:
                    interval = RollingInterval.Infinite;
                    break;
            }

            var path = $"{options.FileLocation}//{options.FileName}.json";
            loggingConfiguration.WriteTo.File(new JsonFormatter(), path, rollingInterval: interval);
        }
    }
}
