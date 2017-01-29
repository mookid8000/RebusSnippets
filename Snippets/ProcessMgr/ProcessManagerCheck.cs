using System.Threading.Tasks;
using NUnit.Framework;
using Rebus.Activation;
using Rebus.Bus;
using Rebus.Config;
using Rebus.Logging;
using Snippets.ProcessMgr.Handlers;
using Snippets.ProcessMgr.Messages;

namespace Snippets.ProcessMgr
{
    [TestFixture]
    public class ProcessManagerCheck : FixtureBase
    {
        [Test]
        public async Task Run()
        {
            StartProcess(ProcessName.ProcA);

            StartProcess(ProcessName.ProcB);

            StartProcess(ProcessName.ProcC);

            var bus = StartProcessManager();

            await bus.SendLocal(new Start());

            await Task.Delay(2000);
        }

        IBus StartProcessManager()
        {
            var activator = new BuiltinHandlerActivator();

            Using(activator);

            activator.Register((bus, context) => new ProcessManager(bus));

            return Configure.With(activator)
                .Logging(l => l.Console(LogLevel.Warn))
                .Transport(t => t.UseMsmq("process_manager"))
                .Start();
        }

        void StartProcess(ProcessName processName)
        {
            var activator = new BuiltinHandlerActivator();

            Using(activator);

            activator.Register((bus, context) => new RequestHandler(bus));

            Configure.With(activator)
                .Logging(l => l.Console(LogLevel.Warn))
                .Transport(t => t.UseMsmq(processName.ToString().ToLowerInvariant()))
                .Start();
        }
    }
}