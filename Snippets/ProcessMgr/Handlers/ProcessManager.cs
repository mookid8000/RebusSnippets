using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Rebus.Bus;
using Rebus.Handlers;
using Rebus.Sagas;
using Snippets.ProcessMgr.Extensions;
using Snippets.ProcessMgr.Messages;
#pragma warning disable 1998

namespace Snippets.ProcessMgr.Handlers
{
    public class ProcessManager : Saga<ProcessManagerState>, IAmInitiatedBy<Start>, IHandleMessages<Reply>
    {
        readonly IBus _bus;

        public ProcessManager(IBus bus)
        {
            _bus = bus;
        }

        protected override void CorrelateMessages(ICorrelationConfig<ProcessManagerState> config)
        {
            config.Correlate<Start>(m => m, d => d.Id);
            config.Correlate<Reply>(m => m.CorrelationId, d => d.Id);
        }

        public async Task Handle(Start message)
        {
            var correlationId = Data.Id.ToString();

            Console.WriteLine($"Starting new process with ID {correlationId}");

            await _bus.SendTo(ProcessName.ProcA, new Request(correlationId, ProcessName.ProcA));

            await _bus.SendTo(ProcessName.ProcB, new Request(correlationId, ProcessName.ProcB));
            
            await _bus.SendTo(ProcessName.ProcC, new Request(correlationId, ProcessName.ProcC));
        }

        public async Task Handle(Reply message)
        {
            Data.HasAnswered.Add(message.Process);

            if (Data.AllProcessesHaveAnswered())
            {
                Console.WriteLine($"Process with ID {Data.Id} is done :)");

                MarkAsComplete();
            }
        }
    }

    public class ProcessManagerState : SagaData
    {
        static readonly ProcessName[] AllProcesses = typeof(ProcessName)
            .GetEnumValues().Cast<ProcessName>()
            .ToArray();

        public HashSet<ProcessName> HasAnswered { get; } = new HashSet<ProcessName>();

        public bool AllProcessesHaveAnswered()
        {
            return !AllProcesses.Except(HasAnswered).Any();
        }
    }

    public enum ProcessName
    {
        ProcA,
        ProcB,
        ProcC,
    }
}

