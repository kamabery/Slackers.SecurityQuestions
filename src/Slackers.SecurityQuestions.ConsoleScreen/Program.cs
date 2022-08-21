
using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.DependencyInjection;
using Slackers.HostedConsole;
using Slackers.Repository.LiteDb;
using Slackers.SecurityQuestions.ConsoleScreen;
using Slackers.SecurityQuestions.ConsoleScreen.Flows;
using Slackers.SecurityQuestions.ConsoleScreen.QuestionService;
using Slackers.SecurityQuestions.ConsoleScreen.State;


var screen = ConsoleScreenAppBuilder.CreateConfigureConsoleScreenApplication(
    "appsettings.json",
    "StructuredLogging",
    "Security Questions", (context, collection) =>
    {
        collection.AddRepository(context.Configuration);
        collection.AddSingleton<IQuestionsService, QuestionsService>();
        collection.AddSingleton<IConsoleScreen, SecurityQuestionScreen>();
        collection.AddSingleton<IStateService<SecurityScreenState>, StateService>();
        collection.AddSingleton<IFlowIoService, FlowIoService>();
        collection.AddTransient<IFlow<SecurityScreenState>, AnswerAttemptFlow>();
        collection.AddTransient<IFlow<SecurityScreenState>, AnswerFlow>();
        collection.AddTransient<IFlow<SecurityScreenState>, MainFlow>();
        collection.AddTransient<IFlow<SecurityScreenState>, SelectQuestionFlow>();
        collection.AddTransient<IFlow<SecurityScreenState>, SelectAnswerFlow>();
        collection.AddTransient<IFlow<SecurityScreenState>, StoreFlow>();
    });

screen.Show();
