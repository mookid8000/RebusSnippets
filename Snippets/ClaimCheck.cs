using System;
using System.IO;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Newtonsoft.Json;
using NUnit.Framework;
using Rebus.Activation;
using Rebus.Config;
using Rebus.DataBus;
using Rebus.DataBus.FileSystem;
using Rebus.Handlers;
using Rebus.Routing.TypeBased;

namespace Snippets
{
    [TestFixture]
    public class ClaimCheck : FixtureBase
    {
        static readonly Encoding FileEncoding = Encoding.UTF8;

        class ProcessFileHandler : IHandleMessages<ProcessFile>
        {
            readonly ManualResetEvent _done;

            public ProcessFileHandler(ManualResetEvent done)
            {
                _done = done;
            }

            public async Task Handle(ProcessFile message)
            {
                var attachment = message.File;

                using (var source = await attachment.OpenRead())
                {
                    using (var streamReader = new StreamReader(source, FileEncoding))
                    {
                        var fileText = await streamReader.ReadToEndAsync();

                        Console.WriteLine(fileText);
                    }
                }

                _done.Set();
            }
        }

        [Test]
        public async Task ItWorks()
        {
            var activator = new BuiltinHandlerActivator();
            var done = new ManualResetEvent(false);

            activator.Register(() => new ProcessFileHandler(done));

            Using(activator);

            Configure.With(activator)
                .Transport(t => t.UseMsmq("claim_check"))
                .Options(o =>
                {
                    o.EnableDataBus()
                        .StoreInFileSystem(@"\\FILESVR004\DataBusStorage");
                })
                .Start();


            var bus = activator.Bus;
            var tempFileName = Path.GetTempFileName();

            File.WriteAllText(tempFileName, @"     ___      ____    ___      ___   
   F __"".   F ___J  F __"".   F __"". 
  J |--\ L J |___: J |--\ L J (___| 
  | |  J | | _____|| |  J | J\___ \ 
  F L__J | F |____JF L__J |.--___) \
 J______/FJ__F    J______/FJ\______J
 |______F |__|    |______F  J______F
                                    
http://patorjk.com/software/taag/#p=testall&f=Graffiti&t=DFDS", FileEncoding);


            using (var sourceStream = File.OpenRead(tempFileName))
            {
                var dataBus = bus.Advanced.DataBus;
                var attachment = await dataBus.CreateAttachment(sourceStream);

                await bus.Send(new ProcessFile(attachment));
            }

            done.WaitOne();
        }

        class ProcessFile
        {
            public DataBusAttachment File { get; }

            public ProcessFile(DataBusAttachment file)
            {
                File = file;
            }
        }

        [Test]
        public void LookAtMessage()
        {
            Console.WriteLine(JsonConvert.SerializeObject(new FullPhysicalNetworkModelExported(new DataBusAttachment(Guid.NewGuid().ToString()),
                new DataBusAttachment(Guid.NewGuid().ToString()),
                new DataBusAttachment(Guid.NewGuid().ToString())), new JsonSerializerSettings
            {
                TypeNameHandling=TypeNameHandling.All,
                Formatting=Formatting.Indented
            }));
        }

        public class FullPhysicalNetworkModelExported
        {
            public FullPhysicalNetworkModelExported(
                DataBusAttachment locations, 
                DataBusAttachment assets, 
                DataBusAttachment equipment)
            {
                Locations = locations;
                Assets = assets;
                Equipment = equipment;
            }

            public DataBusAttachment Locations { get; }

            public DataBusAttachment Assets { get; }

            public DataBusAttachment Equipment { get; }
        }
    }
}