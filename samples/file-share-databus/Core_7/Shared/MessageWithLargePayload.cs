using NServiceBus;
using System;

#region MessageWithLargePayload

//the data bus is allowed to clean up transmitted properties older than the TTBR
[TimeToBeReceived("00:01:00")]
public class MessageWithLargePayload :
    ICommand
{
    public Guid AttachmentID { get; set; }
    public string SomeProperty { get; set; }
    public DataBusProperty<byte[]> LargeBlob { get; set; }
}

#endregion