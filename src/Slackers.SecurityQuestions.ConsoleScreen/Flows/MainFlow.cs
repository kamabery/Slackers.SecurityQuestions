using Microsoft.Extensions.Logging;
using Slackers.HostedConsole;
using Slackers.Logging;
using Slackers.Repository;
using Slackers.SecurityQuestions.ConsoleScreen.Models;
using Slackers.SecurityQuestions.ConsoleScreen.State;

namespace Slackers.SecurityQuestions.ConsoleScreen.Flows;
public class MainFlow : IFlow<SecurityScreenState>
{
    private readonly ILogger<SecurityQuestionScreen> _logger;
    private readonly IFlowIoService _flowIoService;
    private readonly SecurityScreenState _state;
    
    

    public MainFlow(ILogger<SecurityQuestionScreen> logger, IFlowIoService flowIoService, IStateService<SecurityScreenState> stateService)
    {
        _logger = logger;
        _flowIoService = flowIoService;
        _state = stateService.GetState();
        NextFlow = string.Empty;
    }

    public string FlowName => nameof(MainFlow);
   
    public string NextFlow { get; set; }

    public void Run()
    {
        //PO Note: there is no Exit described in the flow, but adding it for UX, I can take it out
        _flowIoService.WriteLine("Hi, what is your name?, or type quit to exit");
        string? displayName = string.Empty;
        while (string.IsNullOrEmpty(displayName))
        {
            displayName = _flowIoService.ReadLine();

            if (string.IsNullOrEmpty(displayName))
            {
                _flowIoService.WriteLine("Please provide a name or type quit");
            }
        }

        var quitString = SecurityQuestionFlows.Quit.ToString();
        
        if (displayName.ToUpper() == quitString.ToUpper())
        {
            NextFlow = quitString;
            return;
        }

        var response = _state.LoadUserFromRepository(displayName);

        if (response == RepositoryResponse.Error)
        {
            _flowIoService.WriteLine("An unexpected error has occurred");
            NextFlow = quitString;
            return;
        }

        // Main flow branch - New User or Answer Questions
        if (response == RepositoryResponse.NotFound)
        {
            // New User
            _logger.LogEvent("New User created", _state.GetLoggingContext());
            NextFlow = SecurityQuestionFlows.StoreFlow.ToString();
            return;
        }

        // Answer attempt flows
        NextFlow = SecurityQuestionFlows.AnswerFlow.ToString();
    }
}