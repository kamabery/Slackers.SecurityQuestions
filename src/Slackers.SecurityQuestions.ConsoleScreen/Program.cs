
using Microsoft.Extensions.DependencyInjection;
using Slackers.SecurityQuestions.ConsoleScreen;


var host = ConsoleProvider.CreateDefaultBuilder(args).Build();
using IServiceScope serviceScope = host.Services.CreateScope();
IServiceProvider provider = serviceScope.ServiceProvider;
var screen = provider.GetRequiredService<IConsoleScreen>();
screen.Show();
