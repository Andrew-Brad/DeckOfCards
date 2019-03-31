using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Threading.Tasks;
using Microsoft.Extensions.DependencyInjection;
using DeckOfCards.Test.Fixtures;
using static DeckOfCards.Test.TestConstants;
using static DeckOfCards.Test.TestExtensions;
using Newtonsoft.Json.Linq;
using Xunit;
using DeckOfCards.Domain;
using Microsoft.Extensions.Configuration;
using System.Linq;

namespace DeckOfCards.Test.DeckQueries
{
    [Collection(SharedServerCollection)]
    public class DeckQueryTests : IAsyncLifetime, IClassFixture<DatabaseConsistentStateFixture>
    {
        private IntegrationTestServerFixture _sharedTestServerFixture;
        private DataProviderFixture _fakeDataFixture;
        private DatabaseConsistentStateFixture _db;

        /// <summary>
        /// The test suite constructor is visible to XUnit and runs once per test.
        /// </summary>
        /// <param name="fixture"></param>
        /// <param name="fakeDataFixture"></param>
        public DeckQueryTests(IntegrationTestServerFixture fixture, DataProviderFixture fakeDataFixture, DatabaseConsistentStateFixture db)
        {
            this._sharedTestServerFixture = fixture;
            this._fakeDataFixture = fakeDataFixture;
            this._db = db;
        }

        public async Task InitializeAsync()
        {
            _db.InitializeFreshDatabase(_sharedTestServerFixture.server.Services.GetRequiredService<IConfiguration>());

            // Seed the traditional 52 card deck templates
            await _fakeDataFixture.SeedCardTemplates(_db.Datastore);

            // Seed some sample deck data
        }

        public async Task DisposeAsync()
        {
            await Task.CompletedTask;
        }

        [Fact]
        public async Task Get_Existing_Deck_Id_Returns_200()
        {
            // Arrange
            await _fakeDataFixture.SeedValidDeck(_db.Datastore);

            // Act
            var response = await _sharedTestServerFixture.HttpClient.GetAsync($"/api/v1/decks/{_fakeDataFixture.Standard52CardDeck.Id}");

            // Assert
            Assert.Equal(System.Net.HttpStatusCode.OK, response.StatusCode);
            JObject responseObject = JObject.Parse(await response.Content.ReadAsStringAsync());
        }

        //[Fact]
        //public async Task Get_0_Card_Deck_Id_Returns_200()
        //{
        //    // Arrange
        //    const string expectedCardRank = "king";
        //    const string expectedCardSuit = "hearts";

        //    // Act
        //    var response = await _sharedTestServerFixture.HttpClient.GetAsync($"/api/v1/cards/templates?rank={expectedCardRank}&suit={expectedCardSuit}");
        //    string responseString = await response.Content.ReadAsStringAsync();

        //    // Assert
        //    Assert.Equal(System.Net.HttpStatusCode.OK, response.StatusCode);
        //    JObject responseObject = JObject.Parse(await response.Content.ReadAsStringAsync());
        //    Assert.Equal(expectedCardRank, responseObject["result"]["rank"].ToString(), ignoreCase: true);
        //    Assert.Equal(expectedCardSuit, responseObject["result"]["suit"].ToString(), ignoreCase: true);
        //    Assert.True(responseObject["result"]["cardName"].ToString().Length > 3);
        //}

        //[Fact]
        //public async Task Get_Nonexistent_Deck_Id_Returns_400()
        //{
        //    // Arrange
        //    string nonexistentId = Guid.NewGuid().ToString();

        //    // Act
        //    var response = await _sharedTestServerFixture.HttpClient.GetAsync($"/api/v1/decks/{nonexistentId}");
        //    string responseString = await response.Content.ReadAsStringAsync();

        //    // Assert
        //    Assert.Equal(System.Net.HttpStatusCode.BadRequest, response.StatusCode);
        //    JObject responseObject = JObject.Parse(await response.Content.ReadAsStringAsync());
        //}

        [Fact]
        public async Task Insanely_Long_String_Does_Not_Pass_Validation() // is someone trying to hack us?
        {
            // Arrange
            string maliciousId = string.Concat(Enumerable.Repeat("lol",500));

            // Act
            var response = await _sharedTestServerFixture.HttpClient.GetAsync($"/api/v1/decks/{maliciousId}");
            string responseString = await response.Content.ReadAsStringAsync();

            // Assert
            Assert.Equal(System.Net.HttpStatusCode.BadRequest, response.StatusCode);
            JObject responseObject = JObject.Parse(await response.Content.ReadAsStringAsync());
        }
    }
}
