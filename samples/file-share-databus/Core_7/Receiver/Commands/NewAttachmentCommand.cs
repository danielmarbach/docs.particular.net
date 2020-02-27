using NServiceBus;
using System;

namespace Receiver.Commands
{
    public class NewAttachmentCommand : ICommand
    {
        public Guid AttachmentID { get; set; }
        
    }
}
