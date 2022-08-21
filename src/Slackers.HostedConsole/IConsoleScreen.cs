namespace Slackers.HostedConsole;

/// <summary>
/// A console screen served by console application.
/// </summary>
public interface IConsoleScreen
{
    string Title { get; }
    void Show();
}