using Slackers.HostedConsole;
using Slackers.SecurityQuestions.ConsoleScreen.Models;
using Slackers.SecurityQuestions.ConsoleScreen.State;

namespace Slackers.SecurityQuestions.ConsoleScreen.Flows;

public class SelectQuestionFlow : IFlow<SecurityScreenState>
{
    private readonly IFlowIoService _flowIoService;
    private readonly SecurityScreenState _state;

    public SelectQuestionFlow(IFlowIoService flowIoService, IStateService<SecurityScreenState> stateService)
    {
        NextFlow = string.Empty;
        _flowIoService = flowIoService;
        _state = stateService.GetState();
    }

    public string FlowName => SecurityQuestionFlows.SelectQuestionFlow;

    public string NextFlow { get; set; }

    public void Run()
    {
        _state.FlowExecuted(FlowName);

        int leftToGo = 3 - _state.AnswerCount;
        var menuQuestions = _state.SecurityQuestions.Select(s => s.Question)
            .Where(s=> 
                !_state.User.Answers.ContainsKey(s)).ToList();

        var menu = MenuManager.AnswerQuestion(
            menuQuestions,
            (menu, question) =>
            {
                _flowIoService.CloseMenu(menu);
                _state.CurrentQuestion = question;
                NextFlow = SecurityQuestionFlows.SelectAnswerFlow;
               
            },
            $"Please select a question. {leftToGo} left to answer");

        _flowIoService.ShowMenu(menu);
    }
}