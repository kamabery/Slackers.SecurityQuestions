using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Slackers.Repository.LiteDb;

namespace Slackers.SecurityQuestions.ConsoleScreen;

public static class ConsoleProvider
{
    public static IHostBuilder CreateDefaultBuilder(string[] strings)
    {
        IHostBuilder hostBuilder1 = Host.CreateDefaultBuilder(strings)
            .ConfigureAppConfiguration(app => { app.AddJsonFile("appsettings.json"); })
            .ConfigureServices((context, services) =>
                {
                    services.AddRepository(context.Configuration);
                    services.AddSingleton<IConsoleScreen, SecurityQuestionScreen>();
                });
                
        return hostBuilder1;
    }


}