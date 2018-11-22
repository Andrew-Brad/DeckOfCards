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
using Raven.Client.Documents.Session;
using Raven.Client.Documents;

namespace DeckOfCards.Test
{
    [Collection(SharedServerCollection)]
    public class RavenDbDebuggingTests : IAsyncLifetime, IClassFixture<DatabaseConsistentStateFixture>
    {
        private IntegrationTestServerFixture _sharedTestServerFixture;
        private DataProviderFixture _fakeDataFixture;
        private DatabaseConsistentStateFixture _db;

        /// <summary>
        /// The test suite constructor is visible to XUnit and runs once per test.
        /// </summary>
        /// <param name="fixture"></param>
        /// <param name="fakeDataFixture"></param>
        public RavenDbDebuggingTests(IntegrationTestServerFixture fixture, DataProviderFixture fakeDataFixture, DatabaseConsistentStateFixture db)
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
        }

        public async Task DisposeAsync()
        {
            await Task.CompletedTask;
        }

        [Fact]
        public async Task Docstore_Debugging()
        {
            using (var session = _db.Datastore.OpenAsyncSession())
            {
                for (int i = 0; i < 5000; i++)
                {
                    PlayingCard card = new PlayingCard(Guid.NewGuid().ToString(), _fakeDataFixture.CardTemplates.First());
                    await session.StoreAsync(card);
                }
                await session.SaveChangesAsync();
            }

            using (var session = _db.Datastore.OpenAsyncSession())
            {
                QueryStatistics queryStats;
                var cardsQuery = session
                    .Query<PlayingCard>()
                    .Include<PlayingCard>(x => x.CardTemplateId)
                    .Statistics(out queryStats)
                    .Where(x => x.Id.StartsWith('a'));
                    


                List<PlayingCard> dbCards = await cardsQuery.ToListAsync();
                CardTemplate template = await session.LoadAsync<CardTemplate>(dbCards.First().CardTemplateId);
            }
        }
    }
}
