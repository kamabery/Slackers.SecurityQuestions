namespace Slackers.SecurityQuestions.ConsoleScreen.State;

public interface IStateService<T>
{
    T GetState();
}