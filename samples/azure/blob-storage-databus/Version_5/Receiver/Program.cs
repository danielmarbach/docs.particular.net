﻿using System;
using NServiceBus;
using NServiceBus.DataBus;

class Program
{
    static void Main()
    {
        BusConfiguration busConfiguration = new BusConfiguration();
        busConfiguration.EndpointName("Sample.AzureBlobStorageDataBus.Receiver");
        busConfiguration.UseSerialization<JsonSerializer>();
        busConfiguration.UseDataBus<AzureDataBus>()
            .ConnectionString("UseDevelopmentStorage=true");
        busConfiguration.UsePersistence<InMemoryPersistence>();
        busConfiguration.EnableInstallers();
        using (IBus bus = Bus.Create(busConfiguration).Start())
        {
            Console.WriteLine("\r\nPress enter key to stop program\r\n");
            Console.Read();
        }
    }
}