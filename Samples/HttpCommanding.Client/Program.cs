﻿using System;
using System.Net.Http;
using System.Threading.Tasks;
using AccidentalFish.Commanding;
using AccidentalFish.Commanding.Http;
using AccidentalFish.DependencyResolver.MicrosoftNetStandard;
using HttpCommanding.Model.Commands;
using HttpCommanding.Model.Results;
using Microsoft.Extensions.DependencyInjection;

namespace HttpCommanding.Client
{
    class Program
    {
        static void Main(string[] args)
        {
            ICommandDispatcher dispatcher = Configure();
            ExecuteHttpCommand(dispatcher);
            Console.ReadKey();            
        }

        static ICommandDispatcher Configure()
        {
            Uri uri = new Uri("http://localhost:52933/api/personalDetails");

            MicrosoftNetStandardDependencyResolver resolver = new MicrosoftNetStandardDependencyResolver(new ServiceCollection());
            ICommandRegistry registry = resolver.UseCommanding(type => resolver.Register(type, type));
            resolver.UseHttpCommanding();
            resolver.BuildServiceProvider();

            IHttpCommandDispatcherFactory httpCommandDispatcherFactory = resolver.Resolve<IHttpCommandDispatcherFactory>();
            registry.Register<UpdatePersonalDetailsCommand>(httpCommandDispatcherFactory.Create(uri, HttpMethod.Put));

            ICommandDispatcher dispatcher = resolver.Resolve<ICommandDispatcher>();
            return dispatcher;
        }

        static async Task ExecuteHttpCommand(ICommandDispatcher dispatcher)
        {            
            UpdateResult result = await dispatcher.DispatchAsync<UpdatePersonalDetailsCommand, UpdateResult>(
                new UpdatePersonalDetailsCommand
                {
                    Age = 10,
                    Forename = "Jim",
                    Surname = "McCoy",
                    Id = Guid.NewGuid()
                });
            Console.WriteLine(result.DidUpdate);
            Console.WriteLine(result.ValidationMessage);
        }
    }
}