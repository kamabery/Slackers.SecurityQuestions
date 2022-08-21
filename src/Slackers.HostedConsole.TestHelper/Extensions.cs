using AutoFixture;

namespace Slackers.HostedConsole.TestHelper;
internal static class Extensions
{
        public static int GetMenuSelection(this IFixture fixture, int min, int max)
        {
            return fixture.Create<int>() % (max - min + 1) + min;
        }
}
 
