using ConsoleTools;

namespace Slackers.HostedConsole
{
    public class FlowIoService : IFlowIoService
    {
        public void WriteLine(string line)
        {
            Console.WriteLine(line);
        }

        public ConsoleKeyInfo ReadKey()
        {
            return Console.ReadKey();
        }

        public string? ReadLine()
        {
            return Console.ReadLine();
        }



        public void ShowMenu(ConsoleMenu menu)
        {
            menu.Show();
        }

        public void CloseMenu(ConsoleMenu menu)
        {
            menu.CloseMenu();
        }

        public void Clear()
        {
            Console.Clear();
        }
    }
}
