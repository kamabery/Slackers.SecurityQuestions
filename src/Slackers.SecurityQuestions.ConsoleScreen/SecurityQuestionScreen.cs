using Microsoft.Extensions.Logging;
using Slackers.HostedConsole;
using Slackers.Logging;
using Slackers.SecurityQuestions.ConsoleScreen.Flows;
using Slackers.SecurityQuestions.ConsoleScreen.QuestionService;
using Slackers.SecurityQuestions.ConsoleScreen.State;

namespace Slackers.SecurityQuestions.ConsoleScreen;

/// <summary>
/// A Console Screen to gather security questions as well as answer them
/// </summary>
public class SecurityQuestionScreen : IConsoleScreen
{
    private readonly ILogger<SecurityQuestionScreen> _logger;
    private readonly IEnumerable<IFlow<SecurityScreenState>> _flows;
    private readonly SecurityScreenState _securityScreenState;
    private readonly IFlowIoService _flowIoService;
    private readonly IQuestionsService _questionsService;


    public SecurityQuestionScreen(
        ILogger<SecurityQuestionScreen> logger, 
        IFlowIoService flowIoService, 
        IStateService<SecurityScreenState> stateService,
        IQuestionsService questionsService,
        IEnumerable<IFlow<SecurityScreenState>> flows)
    {
        _logger = logger;
        _flowIoService = flowIoService;
        _questionsService = questionsService;
        _flows = flows;
        
        _securityScreenState = stateService.GetState();
    }

    public string Title => "Security Questions";

    public void Show()
    {
        if (!_questionsService.LoadSecurityQuestions()) return;
        ShowNextFlow<SecurityQuestionScreen>("MainFlow");
    }

    public void ShowNextFlow<T>(string flowName)
    {
        try
        {
            var flow = _flows.FirstOrDefault(f => f.FlowName == flowName);
            if (flow == null)
            {
                string error = $"Sorry flow {flowName} not found";
                _flowIoService.WriteLine(error);
                _logger.LogEventError(new Exception(error), _securityScreenState.GetLoggingContext());
                return;
            }
            
            flow.NextFlow = string.Empty;
            _securityScreenState.FlowExecuted(flowName);
            flow.Run();
            if (flow.NextFlow == SecurityQuestionFlows.Quit.ToString())
            {
                _logger.LogEvent("Application Finished.", _securityScreenState.GetLoggingContext());
                _flowIoService.WriteLine("Press Any Key to close");
                return;
            }

            if (flow.NextFlow == string.Empty)
            {
                _logger.LogEventError(new Exception("Invalid Flow"), _securityScreenState.GetLoggingContext());
            }

            ShowNextFlow<T>(flow.NextFlow);

        }
        catch (Exception e)
        {
            _logger.LogEventError(e, _securityScreenState.GetLoggingContext());
            _flowIoService.WriteLine("An Error has occurred please close application");
            _flowIoService.ReadKey();
        }
    }
}