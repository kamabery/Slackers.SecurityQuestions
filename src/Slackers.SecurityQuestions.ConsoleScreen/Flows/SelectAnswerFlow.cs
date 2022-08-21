using Microsoft.Extensions.Logging;
using Slackers.HostedConsole;
using Slackers.Logging;
using Slackers.Repository;
using Slackers.SecurityQuestions.ConsoleScreen.State;
using Slackers.SecurityQuestions.Security;

namespace Slackers.SecurityQuestions.ConsoleScreen.Flows;

public class SelectAnswerFlow : IFlow<SecurityScreenState>
{
    private readonly ILogger<SecurityQuestionScreen> _logger;
    private readonly IFlowIoService _flowIoService;
    private readonly SecurityScreenState _state;

  
    public SelectAnswerFlow(
        IFlowIoService flowIoService,
        IStateService<SecurityScreenState> stateService,
        ILogger<SecurityQuestionScreen> logger)
    {
        _logger = logger;
        _flowIoService = flowIoService;
        _state = stateService.GetState();
        NextFlow = string.Empty;
    }

    public string FlowName => SecurityQuestionFlows.SelectAnswerFlow;
    public string NextFlow { get; set; } 
    public void Run()
    {
        _state.FlowExecuted(FlowName);
        _flowIoService.WriteLine("Please Answer");
        var answer = _flowIoService.ReadLine();
        if (string.IsNullOrEmpty(answer))
        {
            _flowIoService.WriteLine("No answer received");
            NextFlow = SecurityQuestionFlows.SelectQuestionFlow;
            return;
        }

        var hashAnswer = answer.Hash();

        if (_state.User == null || string.IsNullOrEmpty(_state.CurrentQuestion))
        {
            var exception = new InvalidOperationException("SelectAnswerFlow in invalid state");
            _logger.LogEventError(exception, _state.GetLoggingContext());
            _flowIoService.WriteLine("An error has occurred");
            NextFlow = SecurityQuestionFlows.Quit;
            return;
        }

        _state.User.Answers.Add(_state.CurrentQuestion, hashAnswer);
        _state.AnswerCount++;
        if (_state.AnswerCount == 3)
        {
            var response = _state.SaveUserToRepository();
            if (response == RepositoryResponse.Created)
            {
                _flowIoService.Clear();
                _state.ResetState();
                NextFlow = SecurityQuestionFlows.MainFlow;
                return;
            }
        }

        _flowIoService.Clear();
       NextFlow = SecurityQuestionFlows.SelectQuestionFlow;
    }
}