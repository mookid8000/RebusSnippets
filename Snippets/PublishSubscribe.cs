using System;
using System.IO;
using System.Threading.Tasks;
using NUnit.Framework;
using Rebus.Activation;
using Rebus.Bus;
using Rebus.Config;
using Rebus.Persistence.FileSystem;
using Rebus.Routing.TypeBased;
#pragma warning disable 1998

namespace Snippets
{
    [TestFixture]
    public class PublishSubscribe : FixtureBase
    {
        [Test]
        public async Task PubSubExample()
        {
            var publisher = GetPublisher();
            var subscriber1 = GetSubscriber(1);
            var subscriber2 = GetSubscriber(2);

            await subscriber1.Subscribe<SomethingHappened>();

            await subscriber2.Subscribe<SomethingHappened>();

            await Task.Delay(1000);

            await publisher.Publish(new SomethingHappened("whatever"));

            await Task.Delay(1000);
        }

        class SomethingHappened
        {
            public string What { get; }

            public SomethingHappened(string what)
            {
                What = what;
            }
        }

        IBus GetSubscriber(int number)
        {
            var activator = new BuiltinHandlerActivator();

            Using(activator);

            activator.Handle<SomethingHappened>(async e =>
            {
                Console.WriteLine($"Something happened: {e.What}");
            });

            Configure.With(activator)
                .Transport(t => t.UseMsmq($"subscriber{number}"))
                .Routing(r => r.TypeBased().Map<SomethingHappened>("publisher"))
                .Start();

            return activator.Bus;
        }

        IBus GetPublisher()
        {
            var activator = new BuiltinHandlerActivator();

            Using(activator);

            var jsonFilePath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "subscriptions.json");

            Configure.With(activator)
                .Transport(t => t.UseMsmq("publisher"))
                .Subscriptions(s => s.UseJsonFile(jsonFilePath))
                .Start();

            return activator.Bus;
        }
    }
}