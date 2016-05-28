﻿using System;
using Events;
using NServiceBus;
using NServiceBus.Features;

class Program
{

    static void Main()
    {
        Console.Title = "Samples.ASB.Polymorphic.Publisher";
        var busConfiguration = new BusConfiguration();
        busConfiguration.EndpointName("Samples.ASB.Polymorphic.Publisher");
        busConfiguration.ScaleOut().UseSingleBrokerQueue();
        var transport = busConfiguration.UseTransport<AzureServiceBusTransport>();
        transport.ConnectionString(Environment.GetEnvironmentVariable("AzureServiceBus.ConnectionString"));
        busConfiguration.UsePersistence<InMemoryPersistence>();
        busConfiguration.UseSerialization<JsonSerializer>();
        busConfiguration.EnableInstallers();
        busConfiguration.DisableFeature<SecondLevelRetries>();

        using (var bus = Bus.Create(busConfiguration).Start())
        {
            Console.WriteLine("Press '1' to publish the base event");
            Console.WriteLine("Press '2' to publish the derived event");
            Console.WriteLine("Press any other key to exit");

            while (true)
            {
                var key = Console.ReadKey();
                Console.WriteLine();

                var eventId = Guid.NewGuid();

                switch (key.Key)
                {
                    case ConsoleKey.D1:
                        bus.Publish<BaseEvent>(e =>
                        {
                            e.EventId = eventId;
                        });
                        Console.WriteLine("BaseEvent sent. EventId: " + eventId);
                        break;

                    case ConsoleKey.D2:
                        bus.Publish<DerivedEvent>(e =>
                        {
                            e.EventId = eventId;
                            e.Data = "more data";
                        });
                        Console.WriteLine("DerivedEvent sent. EventId: " + eventId);
                        break;

                    default:
                        return;
                }
            }
        }
    }
}