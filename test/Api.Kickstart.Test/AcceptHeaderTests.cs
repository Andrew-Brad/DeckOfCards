using System;
using System.Net.Http;
using System.Threading.Tasks;
using System.Xml.Linq;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.AspNetCore.Hosting;
using DeckOfCards.Test.Fixtures;
using static DeckOfCards.Test.TestConstants;
using Newtonsoft.Json.Linq;
using Xunit;
using Raven.Client.Documents;
using Raven.Client.ServerWide.Operations;
using Raven.Client.ServerWide;

namespace DeckOfCards.Test
{
    [Collection(SharedServerCollection)]
    public class AcceptHeaderTests
    {
        private readonly IntegrationTestServerFixture _sharedTestServerFixture;

        public AcceptHeaderTests(IntegrationTestServerFixture fixture)//runs once per unit test
        {
            this._sharedTestServerFixture = fixture;

            CreateDatabase();
        }

        [Fact]
        public async Task Json_Accept_Header_Should_Return_Json()
        {
            //Arrange:
            var client = new HttpClient();
            client.BaseAddress = HostingUri;
            // Act
            var response = await client.GetAsync("/api/v1/cards/templates?rank=king&suit=hearts");
            response.EnsureSuccessStatusCode();
            var responseString = await response.Content.ReadAsStringAsync();
            // Assert
            JObject.Parse(responseString);
        }

        [Fact]
        public async Task Api_Should_Return_406_For_Unacceptable_Header()
        {
            //Arrange
            var client = new HttpClient();
            client.BaseAddress = HostingUri;
            client.SetAcceptHeaderToSomethingStrange();
            // Act
            var response = await client.GetAsync("/api/v1/cards/templates?rank=king&suit=hearts");
            //Assert
            Assert.Equal(System.Net.HttpStatusCode.NotAcceptable, response.StatusCode);
        }

        [Fact]
        public async Task Api_Should_Return_Json_When_No_Accept_Header_Present()
        {
            //Arrange
            var client = new HttpClient();
            client.BaseAddress = HostingUri;
            // Act
            var response = await client.GetAsync("/api/v1/cards/templates?rank=king&suit=hearts");
            response.EnsureSuccessStatusCode();
            var responseString = await response.Content.ReadAsStringAsync();
            // Assert
            JObject.Parse(responseString);
        }

        [Fact]
        public async Task Api_Should_Return_Xml_For_Xml_Accept_Header()
        {
            //Arrange
            var client = new HttpClient();
            client.BaseAddress = HostingUri;
            // Act
            client.SetAcceptHeaderToXml();
            var response = await client.GetAsync("/api/v1/cards/templates?rank=king&suit=hearts");
            response.EnsureSuccessStatusCode();
            var responseString = await response.Content.ReadAsStringAsync();
            var xmlDoc = XDocument.Parse(responseString);
        }

        //[Theory]
        //[InlineData(-1)]
        //[InlineData(0)]
        //[InlineData(1)]
        //public void ReturnFalseGivenValuesLessThan2(int value)
        //{
        //    var result = 1 / value;
        //    Assert.True(result > 0);
        //}

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
    }
}
