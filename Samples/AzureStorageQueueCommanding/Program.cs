﻿using System;
using System.Threading;
using System.Threading.Tasks;
using AccidentalFish.Commanding;
using AccidentalFish.Commanding.Abstractions;
using AccidentalFish.Commanding.Abstractions.Model;
using AccidentalFish.Commanding.AzureStorage;
using AccidentalFish.Commanding.Queue;
using AzureStorageQueueCommanding.Actors;
using AzureStorageQueueCommanding.Commands;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.WindowsAzure.Storage;
using Microsoft.WindowsAzure.Storage.Queue;

namespace AzureStorageQueueCommanding
{
    class Program
    {
        private static IServiceProvider _serviceProvider;

        static void Main(string[] args)
        {
            CancellationTokenSource cancellationTokenSource = new CancellationTokenSource();
#pragma warning disable 4014
            RunDemo(cancellationTokenSource);            
#pragma warning restore 4014
            Console.ReadKey();
            cancellationTokenSource.Cancel();
        }

        private static async Task RunDemo(CancellationTokenSource cancellationTokenSource)
        {
            CloudQueue queue = await ConfigureQueue();
            ConfigureCommanding(queue, out var dispatcher, out var listenerFactory);

#pragma warning disable 4014 // we're just letting things run unmanaged in this console demo
            listenerFactory.Start<OutputToConsoleCommand, DeferredCommandResult>(queue, cancellationTokenSource.Token);
            dispatcher.DispatchAsync(new OutputToConsoleCommand { Message = "Hello" });
#pragma warning restore 4014            
        }

        private static async Task<CloudQueue> ConfigureQueue()
        {
            CloudStorageAccount storageAccount = CloudStorageAccount.DevelopmentStorageAccount;
            CloudQueueClient queueClient = storageAccount.CreateCloudQueueClient();
            CloudQueue queue = queueClient.GetQueueReference("outputtoconsolecommandqueue");
            await queue.CreateIfNotExistsAsync();
            return queue;
        }

        private static void ConfigureCommanding(CloudQueue queue, out ICommandDispatcher dispatcher, out IAzureStorageCommandQueueProcessorFactory listenerFactory)
        {
            ServiceCollection serviceCollection = new ServiceCollection();
            CommandingDependencyResolver dependencyResolver = serviceCollection.GetCommandingDependencyResolver(() => _serviceProvider);
            Options options = new Options
            {
                CommandActorContainerRegistration = type => serviceCollection.AddTransient(type, type)
            };
            ICommandRegistry registry = dependencyResolver.UseCommanding(options);
            dependencyResolver.UseQueues().UseAzureStorageCommanding();

            ICommandDispatcher QueueDispatcher() => _serviceProvider.GetService<IAzureStorageQueueDispatcherFactory>().Create(queue);
            registry
                .Register<OutputToConsoleCommand, DeferredCommandResult, OutputWorldToConsoleCommandActor>(dispatcherFactoryFunc: QueueDispatcher)
                .Register<OutputToConsoleCommand, DeferredCommandResult, OutputBigglesToConsoleCommandActor>();

            _serviceProvider = serviceCollection.BuildServiceProvider();
            dispatcher = _serviceProvider.GetService<ICommandDispatcher>();
            listenerFactory = _serviceProvider.GetService<IAzureStorageCommandQueueProcessorFactory>();
        }
    }
}