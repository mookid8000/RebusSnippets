using System;
using System.Threading.Tasks;
using Rebus.Bus;
using Rebus.Handlers;
using Snippets.ProcessMgr.Messages;

namespace Snippets.ProcessMgr.Handlers
{
    public class RequestHandler : IHandleMessages<Request>
    {
        readonly IBus _bus;

        public RequestHandler(IBus bus)
        {
            _bus = bus;
        }

        public async Task Handle(Request message)
        {
            Console.WriteLine($"Returning reply with correlation ID {message.CorrelationId}");

            await _bus.Reply(new Reply(message.CorrelationId, message.Process));
        }
    }
}