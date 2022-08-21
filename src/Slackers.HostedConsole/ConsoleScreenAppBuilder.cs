using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Slackers.Logging;

namespace Slackers.HostedConsole;

public static class ConsoleScreenAppBuilder
{

    /// <summary>
    /// Creates a hosted console application and returns main console
    /// </summary>
    /// <param name="configurationFile">name of json file</param>
    /// <param name="loggingSection">section in configuration file that contains logging information</param>
    /// <param name="mainScreenTitle">FlowName of main screen</param>
    /// <param name="addServices">action to add services</param>
    /// <returns><see cref="IConsoleScreen"/></returns>
    public static IConsoleScreen CreateConfigureConsoleScreenApplication(
        string configurationFile,
        string loggingSection,
        string mainScreenTitle,
        Action<HostBuilderContext, IServiceCollection> addServices)
    {
        var builder = GetHostBuilder(configurationFile, addServices);

        builder.AddLogging(loggingSection);
        var host = builder.Build();
        var consoleScreen = GetConsoleScreen(mainScreenTitle, host);
        return consoleScreen;
    }

    private static IConsoleScreen GetConsoleScreen(string mainScreenTitle, IHost host)
    {
        using IServiceScope serviceScope = host.Services.CreateScope();
        IServiceProvider provider = serviceScope.ServiceProvider;
        var mainScreen = provider.GetServices<IConsoleScreen>().Where(s => s.Title == mainScreenTitle);

        var consoleScreens = mainScreen as IConsoleScreen[] ?? mainScreen.ToArray();
        if (!consoleScreens.Any() || consoleScreens.Count() > 1)
        {
            Console.WriteLine("Invalid Configuration");
            throw new InvalidOperationException("Invalid Configuration");
        }

        return consoleScreens[0];
    }

    private static IHostBuilder GetHostBuilder(string configurationFile, Action<HostBuilderContext, IServiceCollection> addServices)
    {
        IHostBuilder builder = Host.CreateDefaultBuilder()
            .ConfigureAppConfiguration(app => { app.AddJsonFile(configurationFile); })
            .ConfigureServices(addServices);
        return builder;
    }
}