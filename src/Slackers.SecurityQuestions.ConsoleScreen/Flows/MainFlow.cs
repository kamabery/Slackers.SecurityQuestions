using Microsoft.Extensions.Logging;
using Slackers.HostedConsole;
using Slackers.Logging;
using Slackers.Repository;
using Slackers.SecurityQuestions.ConsoleScreen.State;

namespace Slackers.SecurityQuestions.ConsoleScreen.Flows;
public class MainFlow : IFlow<SecurityScreenState>
{
    private readonly ILogger<SecurityQuestionScreen> _logger;
    private readonly IFlowIoService _flowIoService;
    private readonly SecurityScreenState _state;
    
    

    public MainFlow(IFlowIoService flowIoService, IStateService<SecurityScreenState> stateService, ILogger<SecurityQuestionScreen> logger)
    {
        _logger = logger;
        _flowIoService = flowIoService;
        _state = stateService.GetState();
        NextFlow = string.Empty;
    }

    public string FlowName => SecurityQuestionFlows.MainFlow;
   
    public string NextFlow { get; set; }

    public void Run()
    {
        _state.FlowExecuted(FlowName);

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

        var quitString = SecurityQuestionFlows.Quit;
        
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
            NextFlow = SecurityQuestionFlows.StoreFlow;
            return;
        }

        // Answer attempt flows
        NextFlow = SecurityQuestionFlows.AnswerFlow;
    }
}