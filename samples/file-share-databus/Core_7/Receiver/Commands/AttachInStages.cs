namespace Receiver.Commands
{
    using System;
    using NServiceBus;

    public class AttachInStages : ICommand
    {
        public Guid AttachmentID { get; set; }
        public DataBusProperty<byte[]> LargeBlob { get; set; }
    }
}