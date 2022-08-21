using AutoFixture;
using ConsoleTools;

namespace Slackers.HostedConsole.TestHelper;
public class MockFlowIoService : IFlowIoService
{
    private Fixture _fixture = new();
    
    public Stack<string> UserInput { get; set; } = new();

    public Stack<ConsoleKeyInfo> UserKeyInput { get; set; } = new();
    public Stack<int> UserInputMenuSelection { get; set; } = new();

    public List<string> SystemOutput { get; set; } = new();

    public int ScreenCleared { get; set; }

    public void WriteLine(string line)
    {
        SystemOutput.Add(line);    
    }

    public ConsoleKeyInfo ReadKey()
    {
        if (UserKeyInput.TryPop(out var key))
        {
            return key;
        }

        return  _fixture.Create<ConsoleKeyInfo>();
    }

    public string? ReadLine()
    {
        if (UserInput.TryPop(out var userInput))
        {
            return userInput;
        }

        return _fixture.Create<string>();

    }

    public void ShowMenu(ConsoleMenu menu)
    {
        var actions = new List<Action>();
        foreach (var menuItem in menu.Items)
        {
            actions.Add(menuItem.Action);
        }

        if (UserInputMenuSelection.TryPop(out var menuSelection))
        {
            actions[menuSelection].Invoke();
            return;
        }

        var randomSelection = _fixture.GetMenuSelection(0, actions.Count - 1);
        actions[randomSelection].Invoke();
    }

    public void CloseMenu(ConsoleMenu menu)
    {
        // no op
    }

    public void Clear()
    {
        ScreenCleared++;
    }
}
