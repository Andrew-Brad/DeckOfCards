using System;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using DeckOfCards.Test.Fixtures;
using static DeckOfCards.Test.TestConstants;
using static DeckOfCards.Test.TestExtensions;
using Newtonsoft.Json.Linq;
using Xunit;

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

            // Deck Data
            await _fakeDataFixture.SeedValidDeck(_db.Datastore);
            await _fakeDataFixture.SeedZeroCardDeck(_db.Datastore);
        }

        public async Task DisposeAsync()
        {
            await Task.CompletedTask;
        }

        [Fact]
        public async Task Get_Existing_Deck_Id_Returns_200()
        {
            // Arrange
            string id = _fakeDataFixture.Standard52CardDeck.Id;

            // Act
            var response = await _sharedTestServerFixture.HttpClient.GetAsync($"/api/v1/decks/{id}");
            JObject responseObject = JObject.Parse(await response.Content.ReadAsStringAsync());

            // Assert
            Assert.Equal(System.Net.HttpStatusCode.OK, response.StatusCode);
            Assert.Equal(id, responseObject["result"]["deckId"].ToString());
        }

        [Fact]
        public async Task Get_0_Card_Deck_Id_Returns_200()
        {
            // Arrange
            string id = _fakeDataFixture.ZeroCardDeck.Id;
            // Act
            var response = await _sharedTestServerFixture.HttpClient.GetAsync($"/api/v1/decks/{id}");
            JObject responseObject = JObject.Parse(await response.Content.ReadAsStringAsync());

            // Assert
            Assert.Equal(System.Net.HttpStatusCode.OK, response.StatusCode);
            Assert.Equal(id, responseObject["result"]["deckId"].ToString());
        }

        [Fact]
        public async Task Get_Nonexistent_Deck_Id_Returns_400()
        {
            // Arrange
            string nonexistentId = Guid.NewGuid().ToString();

            // Act
            var response = await _sharedTestServerFixture.HttpClient.GetAsync($"/api/v1/decks/{nonexistentId}");
            JObject responseObject = JObject.Parse(await response.Content.ReadAsStringAsync());

            // Assert
            Assert.Equal(System.Net.HttpStatusCode.NotFound, response.StatusCode);
            // todo - what's expected in the payload?
        }

        [Fact]
        public async Task Insanely_Long_String_Does_Not_Pass_Validation()
        {
            // Arrange
            string maliciousId = string.Concat(Enumerable.Repeat("lol",500));

            // Act
            var response = await _sharedTestServerFixture.HttpClient.GetAsync($"/api/v1/decks/{maliciousId}");
            JObject responseObject = JObject.Parse(await response.Content.ReadAsStringAsync());

            // Assert
            Assert.Equal(System.Net.HttpStatusCode.BadRequest, response.StatusCode);
        }
    }
}
