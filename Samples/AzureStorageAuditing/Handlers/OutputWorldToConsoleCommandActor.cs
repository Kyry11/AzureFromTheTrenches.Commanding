﻿using System;
using System.Threading.Tasks;
using AzureFromTheTrenches.Commanding.Abstractions;
using AzureFromTheTrenches.Commanding.Abstractions.Model;
using AzureStorageAuditing.Commands;

namespace AzureStorageAuditing.Handlers
{
    class OutputWorldToConsoleCommandHandler : ICommandHandler<OutputToConsoleCommand, DeferredCommandResult>
    {
        public Task<DeferredCommandResult> ExecuteAsync(OutputToConsoleCommand command, DeferredCommandResult previousResult)
        {
            Console.WriteLine($"{command.Message} World");            
            return Task.FromResult((DeferredCommandResult)null);
        }
    }
}
