using System;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.DependencyInjection;
using DeckOfCards.Test.Fixtures;
using static DeckOfCards.Test.TestConstants;
using Moq;
using Raven.Client.Documents;
using Xunit;
using Raven.Client.Exceptions;
using DeckOfCards.Queries;
using DeckOfCards.Domain;
using DeckOfCards.QueryHandlers;
using Microsoft.Extensions.Logging;
using AutoMapper;
using Sieve.Services;
using Microsoft.Extensions.Options;
using Sieve.Models;
using Polly.Registry;
using DeckOfCards.CQRS;
using Raven.Client.Documents.Session;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Caching.Memory;

namespace DeckOfCards.Kickstart.Test
{
    [Collection(SharedServerCollection)]
    public class QueryHandlerOutageTests : IAsyncLifetime, IClassFixture<DatabaseConsistentStateFixture>
    {
        private IntegrationTestServerFixture _sharedTestServerFixture;
        private DataProviderFixture _fakeDataFixture;
        private DatabaseConsistentStateFixture _db;

        // Constructor runs once per unit test
        public QueryHandlerOutageTests(IntegrationTestServerFixture fixture, DataProviderFixture fakeDataFixture, DatabaseConsistentStateFixture db)
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

        [Fact(Skip = "Unable to properly override DI to force the specific scenario.")]
        public async Task Handler_Returns_Service_Unavailable_When_Database_Unreachable()
        {
            // Arrange
            var query = new CardTemplateQuery()
            {
                Rank = RanksEnumeration.King,
                Suit = SuitsEnumeration.Hearts
            };
            var docStoreExceptionMock = new Mock<IDocumentStore>();
            var docSessionExceptionMock = new Mock<IAsyncDocumentSession>();

            // Do we mock the policy or RavenDB sessions? https://github.com/App-vNext/Polly/wiki/Unit-testing-with-Polly

            // Lessons learned: even during outages, OpenAsyncSession() does not phone home and will not throw, so it is a poor mock
            // Instead, force Query() to throw, as this will contact the server and bubble up an internal HttpRequest exception
            docSessionExceptionMock.Setup(obj => obj.Query<CardTemplate>(null, null, false))
                .Throws(new RavenException("This exception was thrown from a mock to mimic a database outage."));
            docStoreExceptionMock.Setup(obj => obj.OpenAsyncSession(new SessionOptions() { NoTracking = true }))
                .Returns(docSessionExceptionMock.Object);
            docStoreExceptionMock.Setup(obj => obj.OpenAsyncSession())
                .Returns(docSessionExceptionMock.Object);


            CardTemplateQueryHandler handler = new CardTemplateQueryHandler(
                _sharedTestServerFixture.server.Services.GetRequiredService<ILogger<CardTemplateQueryHandler>>(),
                docStoreExceptionMock.Object,
                //_serverFixture.server.Services.GetRequiredService<IDocumentStore>(),
                _sharedTestServerFixture.server.Services.GetRequiredService<IMapper>(),
                _sharedTestServerFixture.server.Services.GetRequiredService<ISieveProcessor>(),
                _sharedTestServerFixture.server.Services.GetRequiredService<IOptions<SieveOptions>>(),
                _sharedTestServerFixture.server.Services.GetRequiredService<IReadOnlyPolicyRegistry<string>>()
                );

            // Act
            var queryResult = await handler.Handle(query, new System.Threading.CancellationToken());

            // Assert
            Assert.Equal(QueryResultStatus.ServiceUnavailable, queryResult.ResultStatus);
        }
    }
}
