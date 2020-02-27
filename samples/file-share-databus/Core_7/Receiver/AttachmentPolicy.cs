using NServiceBus;
using NServiceBus.Logging;
using Receiver.Commands;
using System;
using System.Threading.Tasks;

namespace Receiver
{
    public class AttachmentPolicy : Saga<AttachmentPolicy.State>,
                                    IAmStartedByMessages<MessageWithLargePayload>,
                                    IHandleMessages<NewAttachmentCommand>,
                                    IHandleMessages<AttachInStages>
    {
        static ILog log = LogManager.GetLogger<AttachmentPolicy>();

        protected override void ConfigureHowToFindSaga(SagaPropertyMapper<State> mapper)
        {
            mapper.ConfigureMapping<MessageWithLargePayload>(message => message.AttachmentID)
                  .ToSaga(saga => saga.AttachmentID);
            mapper.ConfigureMapping<NewAttachmentCommand>(message => message.AttachmentID)
                  .ToSaga(saga => saga.AttachmentID);
            mapper.ConfigureMapping<AttachInStages>(message => message.AttachmentID)
                .ToSaga(saga => saga.AttachmentID);
        }

        public Task Handle(MessageWithLargePayload message, IMessageHandlerContext context)
        {
            log.Info($"Message initially received, size of blob property: {message.LargeBlob.Value.Length} Bytes");

            Data.LargeBlobKey = message.LargeBlob.Key;

            return context.SendLocal<NewAttachmentCommand>(m =>
            {
                m.AttachmentID = message.AttachmentID;
            });
        }

        public Task Handle(NewAttachmentCommand message, IMessageHandlerContext context)
        {
            context.Extensions.Set("LargeBlobKey", Data.LargeBlobKey);

            return context.SendLocal<AttachInStages>(m =>
            {
                m.AttachmentID = message.AttachmentID;
                m.LargeBlob = null;
            });
        }

        public Task Handle(AttachInStages message, IMessageHandlerContext context)
        {
            log.Info($"Message received, size of blob property: {message.LargeBlob.Value.Length} Bytes");
            return Task.CompletedTask;
        }

        public class State : ContainSagaData
        {
            public Guid AttachmentID { get; set; }
            public string LargeBlobKey{ get; set; }
        }
    }
}
