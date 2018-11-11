using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Threading.Tasks;
using System.Linq;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.DependencyInjection;
using DeckOfCards.Test.Fixtures;
using static DeckOfCards.Test.TestConstants;
using static DeckOfCards.Test.TestExtensions;
using Newtonsoft.Json.Linq;
using Xunit;
using Raven.Client.Documents;
using Raven.Client.ServerWide.Operations;
using Raven.Client.ServerWide;
using DeckOfCards.Domain;

namespace DeckOfCards.Test
{
    [Collection(SharedServerCollection)]
    public class DeckCommandTests : IDisposable
    {
        private IntegrationTestServerFixture _sharedTestServerFixture;
        private FakeDataFixture _fakeDataFixture;

        /// <summary>
        /// The test suite constructor is visible to XUnit and runs once per test.
        /// </summary>
        /// <param name="fixture"></param>
        /// <param name="fakeDataFixture"></param>
        public DeckCommandTests(IntegrationTestServerFixture fixture, FakeDataFixture fakeDataFixture)
        {
            this._sharedTestServerFixture = fixture;
            this._fakeDataFixture = fakeDataFixture;

            CreateDatabase();
        }

        [Fact]
        public async Task Create_Deck_No_Post_Body_Returns_201_With_Default_52_Card_Deck()
        {
            // Arrange
            await SeedCardTemplates();

            // Act
            var response = await _sharedTestServerFixture.HttpClient.PostAsync($"/api/v1/decks",null);

            // Assert
            Assert.Equal(System.Net.HttpStatusCode.Created, response.StatusCode);
            string responseString = await response.Content.ReadAsStringAsync();
            JObject responseObject = JObject.Parse(responseString);
            Assert.Equal(52, responseObject["result"]["cards"].Children().Count());
        }


        private async Task MakeValidCards(int count)
        {
            using (var serviceScope = _sharedTestServerFixture.server.Services.CreateScope())
            {
                var docStore = serviceScope.ServiceProvider.GetService<IDocumentStore>();
                using (var session = docStore.OpenAsyncSession())
                {
                    foreach (var item in _fakeDataFixture.ValidCardProvider.Generate(count))
                    {
                        //await session.StoreAsync(item, item.Id.ToString());
                    }
                    await session.SaveChangesAsync();
                }
            }
        }

        private async Task SeedCardTemplates()
        {
            using (var serviceScope = _sharedTestServerFixture.server.Services.CreateScope())
            {
                var docStore = serviceScope.ServiceProvider.GetService<IDocumentStore>();
                using (var session = docStore.OpenAsyncSession())
                {
                    foreach (var cardTemplate in _fakeDataFixture.SourceCardProvider)
                    {
                        await session.StoreAsync(cardTemplate);
                    }
                    await session.SaveChangesAsync();
                }
            }
        }

        private void CreateDatabase()
        {
            DeleteDatabase();
            // Fresh database:
            using (var serviceScope = _sharedTestServerFixture.server.Services.CreateScope())
            {
                var store = serviceScope.ServiceProvider.GetService<IDocumentStore>();
                if (_sharedTestServerFixture.server.Services.GetRequiredService<IHostingEnvironment>().IsDevelopment())
                {
                    store.Maintenance.Server.Send(new CreateDatabaseOperation(new DatabaseRecord("Cards")));
                }
            }
        }

        private void DeleteDatabase()
        {
            // Remove everything from database:
            using (var serviceScope = _sharedTestServerFixture.server.Services.CreateScope())
            {
                var store = serviceScope.ServiceProvider.GetService<IDocumentStore>();
                if (_sharedTestServerFixture.server.Services.GetRequiredService<IHostingEnvironment>().IsDevelopment())
                {
                    store.Maintenance.Server.Send(new DeleteDatabasesOperation("Cards", hardDelete: true));
                }
            }
        }

        public void Dispose() // disposed once per test run (along with constructor)
        {
            DeleteDatabase();
        }
    }
}
