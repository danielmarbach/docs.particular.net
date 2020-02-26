using NServiceBus;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Receiver.Commands
{
    public class NewAttachmentCommand : ICommand
    {
        public Guid AttachmentID { get; set; }
    }
}
