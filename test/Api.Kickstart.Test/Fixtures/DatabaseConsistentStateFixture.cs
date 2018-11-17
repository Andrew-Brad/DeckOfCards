using System;
using System.Threading.Tasks;
using Microsoft.Extensions.Configuration;
using DeckOfCards.WebApi;
using static DeckOfCards.Test.TestConstants;
using Moq;
using Raven.Client.Documents;
using Raven.Client.ServerWide.Operations;
using Raven.Client.ServerWide;
using Raven.Embedded;
using Raven.TestDriver;
using Xunit;

namespace DeckOfCards.Test.Fixtures
{
    /// <summary>
    /// The purpose of this fixture is to instantiate data access objects into a known, consistent state.
    /// Currently, it consumes the same configuration keys as and startup code as the real WebHost (by design).
    /// This fixture is not responsible for staging test data, or imitating outage scenarios.
    /// </summary>
    public class DatabaseConsistentStateFixture : IAsyncLifetime
    {
        public IDocumentStore Datastore { get; private set; }

        public DatabaseConsistentStateFixture()
        {
            // For embedded scenarios:
            //var dbOptions = new TestServerOptions
            //{
            //    DataDirectory = Directory.GetCurrentDirectory(),
            //};

            #region Obsolete sample code
            //Database.ExecuteIndex(new TestDocumentByName());
            //using (var session = Database.OpenSession())
            //{
            //    session.Store(new TestDocument { Name = "Hello world!" });
            //    session.Store(new TestDocument { Name = "Goodbye..." });
            //    session.SaveChanges();
            //}

            ////WaitForIndexing(store); //If we want to query documents sometime we need to wait for the indexes to catch up
            ////WaitForUserToContinueTheTest(store);//Sometimes we want to debug the test itself, this redirect us to the studio
            //using (var session = Database.OpenSession())
            //{
            //    var query = session.Query<TestDocument, TestDocumentByName>().Where(x => x.Name == "hello").ToList();
            //    Assert.Single(query);
            //}
            #endregion
        }

        // this should also have the option of using a TestStartup (or config) to override going out to the network to spool up a DB
        public void InitializeFreshDatabase(IConfiguration config)
        {
            // Remove everything from database:
            Datastore?.Maintenance.Server.Send(new DeleteDatabasesOperation(Datastore.Database, hardDelete: true));
            Datastore = StartupExtensions.InitializeRavenDbDocumentStore(config);
            Datastore.Maintenance.Server.Send(new CreateDatabaseOperation(new DatabaseRecord(config["RavenDb:Database"])));
        }

        public async Task DisposeAsync()
        {
            // Tear down database:
            Datastore.Maintenance.Server.Send(new DeleteDatabasesOperation(Datastore.Database, hardDelete: true));
            await Task.CompletedTask;
        }

        public Task InitializeAsync()
        {
            return Task.CompletedTask;
        }

        //public class TestDocumentByName : AbstractIndexCreationTask<TestDocument>
        //{
        //    public TestDocumentByName()
        //    {
        //        Map = docs => from doc in docs select new { doc.Name };
        //        Indexes.Add(x => x.Name, FieldIndexing.Search);
        //    }
        //}

        //public class TestDocument
        //{
        //    public string Name { get; set; }
        //}
    }
}
