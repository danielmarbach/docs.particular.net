﻿using System;
using NServiceBus;
using NServiceBus.Installation.Environments;
using Shared;

class Program
{
    static void Main()
    {
        Console.Title = "Samples.Gateway.Headquarters";
        var configure = Configure.With();
        configure.Log4Net();
        configure.DefineEndpointName("Samples.Gateway.Headquarters");
        configure.DefaultBuilder();
        configure.RunGatewayWithInMemoryPersistence();
        configure.UseInMemoryGatewayDeduplication();
        configure.InMemorySagaPersister();
        configure.UseInMemoryTimeoutPersister();
        configure.InMemorySubscriptionStorage();
        configure.UseTransport<Msmq>();

        using (var startableBus = configure.UnicastBus().CreateBus())
        {
            var bus = startableBus
                .Start(() => configure.ForInstallationOn<Windows>().Install());
            Console.WriteLine("Press 'Enter' to send a message to RemoteSite which will reply.");
            Console.WriteLine("Press any other key to exit");

            while (true)
            {
                var key = Console.ReadKey();
                Console.WriteLine();

                if (key.Key != ConsoleKey.Enter)
                {
                    return;
                }
                string[] siteKeys =
                {
                    "RemoteSite"
                };
                var priceUpdated = new PriceUpdated
                {
                    ProductId = 2,
                    NewPrice = 100.0,
                    ValidFrom = DateTime.Today,
                };
                bus.SendToSites(siteKeys, priceUpdated);

                Console.WriteLine("Message sent, check the output in RemoteSite");
            }
        }
    }
}