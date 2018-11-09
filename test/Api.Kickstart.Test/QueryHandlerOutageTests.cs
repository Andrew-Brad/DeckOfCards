using System;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.DependencyInjection;
using DeckOfCards.Test.Fixtures;
using static DeckOfCards.Test.TestConstants;
using Moq;
using Raven.Client.Documents;
using Raven.Client.ServerWide.Operations;
using Raven.Client.ServerWide;
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

namespace DeckOfCards.Kickstart.Test
{
    [Collection(SharedServerCollection)]
    public class QueryHandlerOutageTests : IDisposable
    {
        private IntegrationTestServerFixture _serverFixture;
        private FakeDataFixture _fakeDataFixture;

        // Constructor runs once per unit test
        public QueryHandlerOutageTests(IntegrationTestServerFixture fixture, FakeDataFixture fakeDataFixture)
        {
            this._serverFixture = fixture;
            this._fakeDataFixture = fakeDataFixture;
            CreateDatabase();
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
                _serverFixture.server.Services.GetRequiredService<ILogger<CardTemplateQueryHandler>>(),
                docStoreExceptionMock.Object,
                //_serverFixture.server.Services.GetRequiredService<IDocumentStore>(),
                _serverFixture.server.Services.GetRequiredService<IMapper>(),
                _serverFixture.server.Services.GetRequiredService<ISieveProcessor>(),
                _serverFixture.server.Services.GetRequiredService<IOptions<SieveOptions>>(),
                _serverFixture.server.Services.GetRequiredService<IReadOnlyPolicyRegistry<string>>()
                );

            // Act
            var queryResult = await handler.Handle(query, new System.Threading.CancellationToken());

            // Assert
            Assert.Equal(QueryResultStatus.ServiceUnavailable, queryResult.ResultStatus);
        }

        // todo - this method is duplicated multiple times!
        private void CreateDatabase()
        {
            DeleteDatabase();
            // Fresh database:
            using (var serviceScope = _serverFixture.server.Services.CreateScope())
            {
                var store = serviceScope.ServiceProvider.GetService<IDocumentStore>();
                if (_serverFixture.server.Services.GetRequiredService<IHostingEnvironment>().IsDevelopment())
                {
                    store.Maintenance.Server.Send(new CreateDatabaseOperation(new DatabaseRecord("Cards")));
                }
            }
        }

        private void DeleteDatabase()
        {
            // Remove everything from database:
            using (var serviceScope = _serverFixture.server.Services.CreateScope())
            {
                var store = serviceScope.ServiceProvider.GetService<IDocumentStore>();
                if (_serverFixture.server.Services.GetRequiredService<IHostingEnvironment>().IsDevelopment())
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
