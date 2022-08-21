using Microsoft.Extensions.Logging;
using Slackers.HostedConsole;
using Slackers.Logging;
using Slackers.SecurityQuestions.ConsoleScreen.State;

namespace Slackers.SecurityQuestions.ConsoleScreen.Flows;

public class AnswerFlow : IFlow<SecurityScreenState>
{
    private readonly IFlowIoService _flowIoService;
    private readonly ILogger<SecurityQuestionScreen> _logger;
    private readonly SecurityScreenState _state;


    public AnswerFlow(IFlowIoService flowIoService, IStateService<SecurityScreenState> service, ILogger<SecurityQuestionScreen> logger)
    {
        _flowIoService = flowIoService;
        _logger = logger;
        _state = service.GetState();
        NextFlow = string.Empty;
    }

    public string FlowName => SecurityQuestionFlows.AnswerFlow;
    public string NextFlow { get; set; }

    public void Run()
    {
        //PO Note: the specifications were unclear if the user needed to select only from the 
        // questions they answered or from all potential questions.  I went with only the ones they answered,
        // but could display all questions
        _state.FlowExecuted(FlowName);
        if (_state.User == null)
        {
            _flowIoService.WriteLine("System Error Occurred.");
            var exception = new InvalidOperationException("State management failed");
            _logger.LogEventError(exception, _state.GetLoggingContext());
            NextFlow = SecurityQuestionFlows.Quit;
        }

        var attemptsLeft = 3 - _state.AnswerCount;
        var questions = _state.User?.Answers.Keys.ToList();

        if (questions == null || !questions.Any())
        {
            _flowIoService.WriteLine("System Error Occurred.");
            var exception = new InvalidOperationException("State management failed");
            _logger.LogEventError(exception, _state.GetLoggingContext());
            return;
        }

        var menu = MenuManager.AnswerQuestion(
            questions,
            (menu, question) =>
            {
                NextFlow = SecurityQuestionFlows.AnswerAttemptFlow;
                menu.CloseMenu();
                _state.CurrentQuestion = question;
            },
            $"Please Select a question to answer. you have {attemptsLeft} attempts left");
        _flowIoService.ShowMenu(menu);
    }
}