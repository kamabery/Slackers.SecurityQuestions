namespace Slackers.HostedConsole;
public interface IFlow<T>
{
    string FlowName { get; }

    string NextFlow { get; set; }

    void Run();

}


