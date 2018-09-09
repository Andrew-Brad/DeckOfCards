using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.IO;
using System.Net.Http;
using System.Threading.Tasks;
using Xunit;
using static Api.Kickstart.Test.TestConstants;

namespace Api.Kickstart.Test.Fixtures
{
    [CollectionDefinition("SharedServer")]
    public class SharedTestServerCollection : ICollectionFixture<IntegrationTestServerFixture>, ICollectionFixture<FakeDataFixture>
    {
        // This class has no code, and is never created. 
        //Its purpose is simply to be the place to apply [CollectionDefinition] and all the ICollectionFixture<> interfaces.
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

            var webHostBuilder = ApiKickstart.WebApi.Program.CreateWebHostBuilder(null);

            // Unit testing customizations/overrides:
            server = webHostBuilder
                .UseEnvironment("Development")
                .UseContentRoot(pathToContentRoot)
                .UseUrls(HostingUri.ToString())
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