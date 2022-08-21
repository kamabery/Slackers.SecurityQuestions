using Slackers.HostedConsole;
using Slackers.SecurityQuestions.ConsoleScreen.State;

namespace Slackers.SecurityQuestions.ConsoleScreen.Flows;

public class StoreFlow : IFlow<SecurityScreenState>
{
    private readonly IFlowIoService _flowIoService;

    private SecurityScreenState _state;

    public StoreFlow(IFlowIoService flowIoService, IStateService<SecurityScreenState> stateService)
    {
        NextFlow = string.Empty;
        _flowIoService = flowIoService;
        _state = stateService.GetState();
    }

    public string FlowName => SecurityQuestionFlows.StoreFlow;
    public string NextFlow { get; set; }

    public void Run()
    {
        _state.FlowExecuted(FlowName);
        var menu = MenuManager.YesNo(
            () => NextFlow = SecurityQuestionFlows.SelectQuestionFlow,
            () => NextFlow = SecurityQuestionFlows.MainFlow,
            "Would you like to store answers to security questions?");

        _flowIoService.ShowMenu(menu);
    }
}