﻿namespace Core6
{
    using System.Threading.Tasks;
    using NServiceBus;

    class BasicUsageOfIBus
    {
        async Task Send(EndpointConfiguration endpointConfiguration)
        {
            #region BasicSend
            var endpointInstance = await Endpoint.Start(endpointConfiguration)
                .ConfigureAwait(false);
            await endpointInstance.Send(new MyMessage())
                .ConfigureAwait(false);
            #endregion
        }

        #region SendFromHandler

        public class MyMessageHandler : IHandleMessages<MyMessage>
        {
            public Task Handle(MyMessage message, IMessageHandlerContext context)
            {
                return context.Send(new OtherMessage());
            }
        }
        #endregion

        async Task SendInterface(IEndpointInstance endpoint)
        {
            #region BasicSendInterface
            await endpoint.Send<IMyMessage>(m => m.MyProperty = "Hello world")
                .ConfigureAwait(false);
            #endregion
        }

        async Task SetDestination(IEndpointInstance endpoint)
        {
            #region BasicSendSetDestination
            var options = new SendOptions();
            options.SetDestination("MyDestination");
            await endpoint.Send(new MyMessage(), options)
                .ConfigureAwait(false);
            //or
            await endpoint.Send<MyMessage>("MyDestination", m => { })
                .ConfigureAwait(false);
            #endregion
        }

        async Task SpecificInstance(IEndpointInstance endpoint)
        {
            #region BasicSendSpecificInstance
            var options = new SendOptions();
            options.RouteToSpecificInstance("MyInstance");
            await endpoint.Send(new MyMessage(), options)
                .ConfigureAwait(false);
            #endregion
        }

        async Task ThisEndpoint(IEndpointInstance endpoint)
        {
            #region BasicSendToAnyInstance
            var options = new SendOptions();
            options.RouteToThisEndpoint();
            await endpoint.Send(new MyMessage(), options)
                .ConfigureAwait(false);
            //or
            await endpoint.SendLocal(new MyMessage())
                .ConfigureAwait(false);
            #endregion
        }

        async Task ThisInstance(IEndpointInstance endpoint)
        {
            #region BasicSendToThisInstance
            var options = new SendOptions();
            options.RouteToThisInstance();
            await endpoint.Send(new MyMessage(), options)
                .ConfigureAwait(false);
            #endregion
        }

        async Task SendReplyToThisInstance(IEndpointInstance endpoint)
        {
            #region BasicSendReplyToThisInstance
            var options = new SendOptions();
            options.RouteReplyToThisInstance();
            await endpoint.Send(new MyMessage(), options)
                .ConfigureAwait(false);
            #endregion
        }

        async Task SendReplyToAnyInstance(IEndpointInstance endpoint)
        {
            #region BasicSendReplyToAnyInstance
            var options = new SendOptions();
            options.RouteReplyToAnyInstance();
            await endpoint.Send(new MyMessage(), options)
                .ConfigureAwait(false);
            #endregion
        }

        async Task SendReplyTo(IEndpointInstance endpoint)
        {
            #region BasicSendReplyToDestination
            var options = new SendOptions();
            options.RouteReplyTo("MyDestination");
            await endpoint.Send(new MyMessage(), options)
                .ConfigureAwait(false);
            #endregion
        }

        async Task ReplySendReplyToThisInstance(IMessageHandlerContext context)
        {
            #region BasicReplyReplyToThisInstance
            var options = new ReplyOptions();
            options.RouteReplyToThisInstance();
            await context.Reply(new MyMessage(), options)
                .ConfigureAwait(false);
            #endregion
        }

        async Task ReplySendReplyToAnyInstance(IMessageHandlerContext context)
        {
            #region BasicReplyReplyToAnyInstance
            var options = new ReplyOptions();
            options.RouteReplyToAnyInstance();
            await context.Reply(new MyMessage(), options)
                .ConfigureAwait(false);
            #endregion
        }

        async Task ReplySendReplyTo(IMessageHandlerContext context)
        {
            #region BasicReplyReplyToDestination
            var options = new ReplyOptions();
            options.RouteReplyTo("MyDestination");
            await context.Reply(new MyMessage(), options)
                .ConfigureAwait(false);
            #endregion
        }

        public class MyMessage
        {
        }

        public class OtherMessage
        {
        }

        interface IMyMessage
        {
            string MyProperty { get; set; }
        }
    }
}
