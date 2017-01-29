using System;
using System.Threading.Tasks;
using NUnit.Framework;
using Rebus.Activation;
using Rebus.Bus;
using Rebus.Config;
using Rebus.Handlers;
using Rebus.Routing;
using Rebus.Routing.TypeBased;
using Snippets.Messages;
// ReSharper disable ArgumentsStyleAnonymousFunction

namespace Snippets
{
    [TestFixture]
    public class RequestReply : FixtureBase
    {
        [Test]
        public async Task LeaveMessageInQueue()
        {
            var requestor = GetRequestor();

            GetReplier();

            await requestor.Send(new GetGreetingRequest("Don Jon"));
        }


        IBus GetRequestor()
        {
            var activator = new BuiltinHandlerActivator();

            Using(activator);

            Configure.With(activator)
                .Transport(t => t.UseMsmq("requestor"))
                .Routing(r => r.TypeBased().Map<GetGreetingRequest>("replier"))
                .Start();

            return activator.Bus;
        }

        IBus GetReplier()
        {
            var activator = new BuiltinHandlerActivator();

            Using(activator);

            activator.Register((bus, context) => new GetGreetingRequestHandler(bus));

            Configure.With(activator)
                .Transport(t => t.UseMsmq("requestor"))
                .Routing(r => r.TypeBased().Map<GetGreetingRequest>("replier"))
                .Start();

            return activator.Bus;
        }
    }

    class GetGreetingRequestHandler : IHandleMessages<GetGreetingRequest>
    {
        readonly IBus _bus;

        public GetGreetingRequestHandler(IBus bus)
        {
            _bus = bus;
        }

        public async Task Handle(GetGreetingRequest message)
        {
            var reply = new GetGreetingReply($"Hello {message.Name}");

            await _bus.Reply(reply);
        }
    }

    namespace Messages
    {
        public class GetGreetingRequest
        {
            public string Name { get; }

            public GetGreetingRequest(string name)
            {
                Name = name;
            }
        }

        public class GetGreetingReply
        {
            public string Greeting { get; }

            public GetGreetingReply(string greeting)
            {
                Greeting = greeting;
            }
        }
    }
}