using System.Threading.Tasks;
using Rebus.Bus;
using Snippets.ProcessMgr.Handlers;

namespace Snippets.ProcessMgr.Extensions
{
    public static class BusExtensions
    {
        public static async Task SendTo(this IBus bus, ProcessName destination, object message)
        {
            await bus.Advanced.Routing.Send(destination.ToString().ToLowerInvariant(), message);
        }
    }
}