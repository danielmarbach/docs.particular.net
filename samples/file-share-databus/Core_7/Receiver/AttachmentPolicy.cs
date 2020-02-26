using NServiceBus;
using NServiceBus.Logging;
using Receiver.Commands;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Receiver
{
    public class AttachmentPolicy : Saga<AttachmentPolicy.State>,
                                    IAmStartedByMessages<MessageWithLargePayload>,
                                    IHandleMessages<NewAttachmentCommand>
    {
        static ILog log = LogManager.GetLogger<AttachmentPolicy>();

        protected override void ConfigureHowToFindSaga(SagaPropertyMapper<State> mapper)
        {
            mapper.ConfigureMapping<MessageWithLargePayload>(message => message.AttachmentID)
                  .ToSaga(saga => saga.AttachmentID);
            mapper.ConfigureMapping<NewAttachmentCommand>(message => message.AttachmentID)
                  .ToSaga(saga => saga.AttachmentID);
        }

        public Task Handle(MessageWithLargePayload message, IMessageHandlerContext context)
        {
            Data.LargeBlob = message.LargeBlob;

            return context.SendLocal<NewAttachmentCommand>(m =>
            {
                m.AttachmentID = message.AttachmentID;
            });
        }

        public Task Handle(NewAttachmentCommand message, IMessageHandlerContext context)
        {
            log.Info($"Message received, size of blob property: {Data.LargeBlob.Value.Length} Bytes");
            return Task.CompletedTask;
        }

        public class State : ContainSagaData
        {
            public Guid AttachmentID { get; set; }
            public DataBusProperty<byte[]> LargeBlob { get; set; }
        }
    }
}
