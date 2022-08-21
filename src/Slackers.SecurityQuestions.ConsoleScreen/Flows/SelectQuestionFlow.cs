using Slackers.HostedConsole;
using Slackers.SecurityQuestions.ConsoleScreen.State;

namespace Slackers.SecurityQuestions.ConsoleScreen.Flows;

public class SelectQuestionFlow : IFlow<SecurityScreenState>
{
    private readonly IFlowIoService _flowIoService;
    private readonly SecurityScreenState _state;

    public SelectQuestionFlow(IFlowIoService flowIoService, IStateService<SecurityScreenState> stateService)
    {
        _flowIoService = flowIoService;
        _state = stateService.GetState();
    }

    public string FlowName => nameof(SelectQuestionFlow);

    public string NextFlow { get; set; }

    public void Run()
    {
        int leftToGo = 3 - _state.AnswerCount;
        var menuQuestions = _state.SecurityQuestions.Select(s => s.Question).ToList();
        var menu = MenuManager.AnswerQuestion(
            menuQuestions,
            (menu, question) =>
            {
                _state.CurrentQuestion = question;
                NextFlow = SecurityQuestionFlows.SelectAnswerFlow.ToString();
                _flowIoService.CloseMenu(menu);
            },
            $"Please select a question. {leftToGo} left to answer");

        _flowIoService.ShowMenu(menu);
    }
}