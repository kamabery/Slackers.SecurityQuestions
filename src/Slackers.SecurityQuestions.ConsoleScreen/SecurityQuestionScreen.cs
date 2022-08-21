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
        IQuestionsService questionsService)
    {
        _logger = logger;
        _flowIoService = flowIoService;
        _questionsService = questionsService;
        _flows = new List<IFlow<SecurityScreenState>>
        {
            // This could be done via IOC however that makes the integration test more brittle
            new MainFlow(_flowIoService, stateService, logger),
            new AnswerFlow(flowIoService, stateService, logger),
            new AnswerAttemptFlow(_flowIoService, stateService, logger),
            new SelectAnswerFlow(flowIoService, stateService, logger),
            new SelectQuestionFlow(flowIoService, stateService),
            new StoreFlow(flowIoService, stateService)
        };
    }

    public string Title => "Security Questions";

    public void Show()
    {
        if (!_questionsService.LoadSecurityQuestions()) return;
        ShowNextFlow<SecurityQuestionScreen, SecurityScreenState>(
            _flows,
            SecurityQuestionFlows.MainFlow, 
            SecurityQuestionFlows.Quit);
    }

    public void ShowNextFlow<T,TT>(IEnumerable<IFlow<TT>> flows, string flowName, string finalFlow)
    {
        try
        {
            var flow = _flows.FirstOrDefault(f => f.FlowName == flowName);
            if (flow == null)
            {
                string error = $"Sorry flow {flowName} not found";
                _flowIoService.WriteLine(error);
                _logger.LogError(error);
                return;
            }
            
            flow.NextFlow = string.Empty;
            // state.FlowExecuted(flowName);
            flow.Run();
            if (flow.NextFlow == finalFlow)
            {
                return;
            }

            if (flow.NextFlow == string.Empty)
            {
                _logger.LogError("Invalid Flow");
                return;
            }

            ShowNextFlow<T,TT>(flows, flow.NextFlow, finalFlow);
        }
        catch (Exception e)
        {
            _logger.LogEventError(e, _securityScreenState.GetLoggingContext());
            _flowIoService.WriteLine("An Error has occurred please close application");
            _flowIoService.ReadKey();
        }
    }
}