using System;
using System.Data.SqlClient;
using System.Linq;
using System.Threading.Tasks;
using NServiceBus;
using NServiceBus.Pipeline;
using Receiver.Commands;

class Program
{
    static async Task Main()
    {
        var endpointName = "Samples.DataBus.Receiver";
        Console.Title = endpointName;



        var endpointConfiguration = new EndpointConfiguration(endpointName);
        
        var dataBus = endpointConfiguration.UseDataBus<FileShareDataBus>();
        dataBus.BasePath(@"..\..\..\..\storage");

        endpointConfiguration.Pipeline.Register(new Behavior.Registration());

        var persistence = endpointConfiguration.UsePersistence<LearningPersistence>();
        //var persistence = endpointConfiguration.UsePersistence<SqlPersistence>();
        var transport = endpointConfiguration.UseTransport<LearningTransport>();

        //endpointConfiguration.EnableInstallers();

        //#region SAGA Persistence
        //persistence.SqlDialect<SqlDialect.MsSqlServer>();
        //persistence.ConnectionBuilder(
        //    connectionBuilder: () =>
        //    {
        //        return new SqlConnection(@"Data Source=localhost\SQLEXPRESS;Initial Catalog=AttachmentPersistanceIssueBrynjar;Integrated Security=True");
        //    });
        //var subscriptions = persistence.SubscriptionSettings();
        //subscriptions.CacheFor(TimeSpan.FromMinutes(1));
        //#endregion

        #region Routing
        var routing = transport.Routing();
        routing.RouteToEndpoint(typeof(NewAttachmentCommand).Assembly,
            "Receiver.Commands",
            endpointName);
        #endregion Routing

        var endpointInstance = await Endpoint.Start(endpointConfiguration)
            .ConfigureAwait(false);
        
        
        Console.WriteLine("Press any key to exit");
        Console.ReadKey();
        await endpointInstance.Stop()
            .ConfigureAwait(false);
    }

    class Behavior : Behavior<IOutgoingLogicalMessageContext>
    {
        public override Task Invoke(IOutgoingLogicalMessageContext context, Func<Task> next)
        {
            if (context.Message.Instance is NewAttachmentCommand command && context.TryGetIncomingPhysicalMessage(out var incomingMessage))
            {
                var kvp = incomingMessage.Headers.Single(x => x.Key.StartsWith("NServiceBus.DataBus."));
                context.Headers[kvp.Key] = kvp.Value;

                command.LargeBlob = new DataBusProperty<byte[]> { Key = kvp.Value };
            }
            return next();
        }

        public class Registration : RegisterStep
        {
            public Registration() : base("DataBusSendHack", typeof(Behavior), "Readds the databus property")
            {
                InsertAfter("DataBusSend");
            }
        }
    }
}