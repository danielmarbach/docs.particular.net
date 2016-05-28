﻿namespace Core4.Headers
{
    using NServiceBus;
    using NServiceBus.Saga;

    #region header-outgoing-saga
    public class WriteSaga : Saga<WriteSagaData>,
        IHandleMessages<MyMessage>
    {
        public void Handle(MyMessage message)
        {
            var someOtherMessage = new SomeOtherMessage();
            Bus.SetMessageHeader(someOtherMessage, "MyCustomHeader", "My custom value");
            Bus.Send(someOtherMessage);
        }
    }

    #endregion
    public class WriteSagaData : ContainSagaData
    {
    }

}
