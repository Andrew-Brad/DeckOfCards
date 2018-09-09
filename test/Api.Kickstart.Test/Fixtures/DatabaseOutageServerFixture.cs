using System;
using System.IO;
using System.Net.Http;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.DependencyInjection;
using Moq;
using Raven.Client.Documents;
using Raven.Client.Exceptions;
using Xunit;
using static Api.Kickstart.Test.TestConstants;

namespace Api.Kickstart.Test.Fixtures
{
    [CollectionDefinition(DatabaseOutageServerCollection)]
    public class DatabaseOutageServerFixtureCollection : ICollectionFixture<DatabaseOutageServerFixture>, ICollectionFixture<FakeDataFixture>
    {
        // This class has no code, and is never created. 
        // Its purpose is simply to be the place to apply [CollectionDefinition] and all the ICollectionFixture<> interfaces.
    }

    [Obsolete("There is currently an unresolved issue with running multiple Web fixtures together.  Research AppDomains and xunit.runner.json. Setting threads to 1 has helped (but not solved) the issue.")]
    public class DatabaseOutageServerFixture : IAsyncLifetime
    {
        public readonly IWebHost server;
        public readonly HttpClient HttpClient;

        public DatabaseOutageServerFixture()
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
                .ConfigureServices(services =>
                {
                    // Override DI for scenarios that represent outages and unhandled errors
                    var docStoreExceptionMock = new Mock<IDocumentStore>();
                    docStoreExceptionMock.Name = "DB Down Mock.";                    
                    //docStoreExceptionMock.CallBase = true;
                    docStoreExceptionMock.Setup(obj => obj.OpenAsyncSession()).Throws(new RavenException("This exception was thrown from a mock to mimic a database outage."));
                    services.AddSingleton<IDocumentStore>(docStoreExceptionMock.Object);

                })
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
