using DeckOfCards.WebApi;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;
using System.IO;
using System.Net.Http;
using System.Threading.Tasks;
using Xunit;
using static DeckOfCards.Test.TestConstants;

namespace DeckOfCards.Test.Fixtures
{
    [CollectionDefinition(SharedServerCollection)]
    public class SharedTestServerCollection : ICollectionFixture<IntegrationTestServerFixture>, ICollectionFixture<DataProviderFixture>
    {
        // This class has no code, and is never created. 
        // Its purpose is simply to be the place to apply [CollectionDefinition] and all the ICollectionFixture<> interfaces.
    }

    public class IntegrationTestServerFixture : IAsyncLifetime
    {
        public readonly IWebHost server;
        public readonly HttpClient HttpClient;

        public IntegrationTestServerFixture()
        {
            // Prep unit testing variables and WebServer customizations - https://docs.microsoft.com/en-us/aspnet/core/testing/integration-testing
            string unitTestDirectory = Directory.GetCurrentDirectory();
            string pathToContentRoot = Path.GetFullPath(Path.Combine(unitTestDirectory, NavigationPathDirectoryToApi));

            var webHostBuilder = DeckOfCards.WebApi.Program.CreateWebHostBuilder(null);

            // Unit testing customizations/overrides:
            server = webHostBuilder
                .UseEnvironment("Development")
                .UseContentRoot(pathToContentRoot)
                .UseUrls(HostingUri.ToString())
                .ConfigureAppConfiguration((hostingContext, config) =>
                {
                    // Config overrides from any number of sources
                    var unitTestingOverrides = new List<KeyValuePair<string, string>>()
                        {
                            new KeyValuePair<string, string>(StartupExtensions.ConfigKeyRavenDbDatabase,Guid.NewGuid().ToString()),                           
                        };
                    config.AddInMemoryCollection(unitTestingOverrides);
                })
                //.UseStartup<TestStartup>() // Mediatr + Automapper are using GetType() and AppDomains, which won't work with this convention
                .Build();

            server.Start();

            // Init client:
            HttpClient = new HttpClient();
            HttpClient.BaseAddress = HostingUri;
            HttpClient.SetAcceptHeaderToJson();
        }

        public async Task DisposeAsync()
        {
            await server.StopAsync();
            HttpClient.Dispose();
            server.Dispose();
        }

        public Task InitializeAsync()
        {
            return Task.CompletedTask;
        }
    }
}