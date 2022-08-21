using Slackers.HostedConsole;
using Slackers.SecurityQuestions.ConsoleScreen.State;

namespace Slackers.SecurityQuestions.ConsoleScreen.Flows;

public class StoreFlow : IFlow<SecurityScreenState>
{
    private readonly IFlowIoService _flowIoService;

    public StoreFlow(IFlowIoService flowIoService, IStateService<SecurityScreenState> stateService)
    {
        _flowIoService = flowIoService;
    }

    public string FlowName => nameof(StoreFlow);
    public string NextFlow { get; set; }

    public void Run()
    {
        var menu = MenuManager.YesNo(
            () => NextFlow = "SelectQuestionFlow",
            () => NextFlow = "MainFlow",
            "Would you like to store answers to security questions?");

        _flowIoService.ShowMenu(menu);
    }
}