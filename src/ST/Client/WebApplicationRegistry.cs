﻿using System.IO;
using StoryTeller.Commands;
using StoryTeller.Files;
using StructureMap;

namespace ST.Client
{
    public class WebApplicationRegistry : Registry
    {
        public WebApplicationRegistry(string webSocketAddress, WebSocketsHandler webSockets, IRemoteController controller)
        {
            For<IRemoteController>().Use(controller);

            ForSingletonOf<IClientConnector>()
                .Use<ClientConnector>()
                .SetProperty(x => x.WebSocketsAddress = webSocketAddress);

            For<WebSocketsHandler>().Use(webSockets);

            For<ISpecFileWatcher>().Use<SpecFileWatcher>();
            For<IFixtureFileWatcher>().Use<FixtureFileWatcher>();

            ForSingletonOf<AssetFileWatcher>().Use<AssetFileWatcher>();

            ForSingletonOf<IPersistenceController>().Use<PersistenceController>();
            ForSingletonOf<IFixtureController>().Use<FixtureController>();

            For<IApplicationFiles>().Use(new ApplicationFiles(Directory.GetCurrentDirectory()));

            Scan(x =>
            {
                x.AssemblyContainingType<ICommand>();
                x.AssemblyContainingType<OpenInput>();

                x.AddAllTypesOf<ICommand>();
            });
        }
    }
}
