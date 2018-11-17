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

namespace DeckOfCards.Test
{
    [Collection(SharedServerCollection)]
    public class CardTemplateQueryTests : IAsyncLifetime, IClassFixture<DatabaseConsistentStateFixture>
    {
        private IntegrationTestServerFixture _sharedTestServerFixture;
        private DataProviderFixture _fakeDataFixture;
        private DatabaseConsistentStateFixture _db;

        /// <summary>
        /// The test suite constructor is visible to XUnit and runs once per test.
        /// </summary>
        /// <param name="fixture"></param>
        /// <param name="fakeDataFixture"></param>
        public CardTemplateQueryTests(IntegrationTestServerFixture fixture, DataProviderFixture fakeDataFixture, DatabaseConsistentStateFixture db)
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
        public async Task Get_Card_Template_Returns_200()
        {
            // Arrange
            const string expectedCardRank = "king";
            const string expectedCardSuit = "hearts";

            // Act
            var response = await _sharedTestServerFixture.HttpClient.GetAsync($"/api/v1/cards/templates?rank={expectedCardRank}&suit={expectedCardSuit}");
            
            // Assert
            Assert.Equal(System.Net.HttpStatusCode.OK, response.StatusCode);
            JObject responseObject = JObject.Parse(await response.Content.ReadAsStringAsync());
            Assert.Equal(expectedCardRank,responseObject["result"]["rank"].ToString(),ignoreCase: true);
            Assert.Equal(expectedCardSuit, responseObject["result"]["suit"].ToString(), ignoreCase: true);
            Assert.True(responseObject["result"]["cardName"].ToString().Length > 3);
        }

        [Fact]
        public async Task Get_Card_Template_Alternate_Url_Returns_200()
        {
            // Arrange
            const string expectedCardRank = "king";
            const string expectedCardSuit = "hearts";

            // Act
            var response = await _sharedTestServerFixture.HttpClient.GetAsync($"/api/v1/cards/templates/{expectedCardRank}/{expectedCardSuit}");

            // Assert
            Assert.Equal(System.Net.HttpStatusCode.OK, response.StatusCode);
            JObject responseObject = JObject.Parse(await response.Content.ReadAsStringAsync());
            Assert.Equal(expectedCardRank, responseObject["result"]["rank"].ToString(), ignoreCase: true);
            Assert.Equal(expectedCardSuit, responseObject["result"]["suit"].ToString(), ignoreCase: true);
            Assert.True(responseObject["result"]["cardName"].ToString().Length > 3);
        }

        [Fact]
        public async Task Get_Card_Template_Invalid_Returns_Well_Formatted_400()
        {
            // Arrange
            const string expectedCardRank = "king";
            const string expectedCardSuit = "junk";

            // Act
            var response = await _sharedTestServerFixture.HttpClient.GetAsync($"/api/v1/cards/templates/{expectedCardSuit}/{expectedCardRank}");
            string responseString = await response.Content.ReadAsStringAsync();

            // Assert
            Assert.Equal(System.Net.HttpStatusCode.BadRequest, response.StatusCode);
            //JObject responseObject = JObject.Parse(await response.Content.ReadAsStringAsync());
            //Assert.Equal(expectedCardRank, responseObject["result"]["rank"].ToString(), ignoreCase: true);
            //Assert.Equal(expectedCardSuit, responseObject["result"]["suit"].ToString(), ignoreCase: true);
            //Assert.True(responseObject["result"]["cardName"].ToString().Length > 3);
        }

        //[Fact]
        //public async Task Get_Widgets_No_Url_Paging_Defaults_Correctly()
        //{
        //    // Arrange
        //    int totalInDataset = 220;
        //    int defaultPageSize = 100; // should respect config value of 100
        //    await MakeValidWidgets(totalInDataset);
        //    // Act
        //    var response = await _sharedTestServerFixture.HttpClient.GetAsync("/api/v1/widgets");
        //    // Assert
        //    Assert.Equal(System.Net.HttpStatusCode.OK, response.StatusCode);
        //    var responseObject = JObject.Parse(await response.Content.ReadAsStringAsync());
        //    var widgets = responseObject["result"]["widgets"].ToObject<List<Widget>>();
        //    Assert.Equal(defaultPageSize, widgets.Count);
        //    Assert.All(widgets, item => Assert.True(!string.IsNullOrEmpty(item.Id)));
        //    Assert.All(widgets, item => Assert.True(item.WidgetName.Length > 1));
        //}

        //[Fact] // Note: not generalized for N pages
        //public async Task Get_Widgets_Respects_Paging()
        //{
        //    // Arrange 
        //    int totalInDataset = 80;
        //    int pageSize = 33;
        //    await MakeValidWidgets(totalInDataset);
        //    // Act
        //    var response1 = _sharedTestServerFixture.HttpClient.GetAsync($"/api/v1/widgets?page={1}&pageSize={pageSize}");
        //    var response2 = _sharedTestServerFixture.HttpClient.GetAsync($"/api/v1/widgets?page={2}&pageSize={pageSize}");
        //    var response3 = _sharedTestServerFixture.HttpClient.GetAsync($"/api/v1/widgets?page={3}&pageSize={pageSize}"); // intentional coverage of edge case
        //    await Task.WhenAll(response1, response2, response3);
        //    var responseObject1 = JObject.Parse(await response1.Result.Content.ReadAsStringAsync());
        //    var responseObject2 = JObject.Parse(await response2.Result.Content.ReadAsStringAsync());
        //    var responseObject3 = JObject.Parse(await response3.Result.Content.ReadAsStringAsync());
        //    var widgets1 = responseObject1["result"]["widgets"].ToObject<List<Widget>>();
        //    var widgets2 = responseObject2["result"]["widgets"].ToObject<List<Widget>>();
        //    var widgets3 = responseObject3["result"]["widgets"].ToObject<List<Widget>>();
        //    // Assert
        //    Assert.All(widgets1.Union(widgets2).Union(widgets3), item => Assert.True(!string.IsNullOrEmpty(item.Id)));
        //    Assert.All(widgets1.Union(widgets2).Union(widgets3), item => Assert.True(item.WidgetName.Length > 1));
        //    Assert.Equal(pageSize, widgets1.Count);
        //    Assert.Equal(pageSize, widgets2.Count);
        //    Assert.Equal(totalInDataset - (pageSize * 2), widgets3.Count);
        //}

        //[Fact]
        //public async Task Get_Widgets_Respects_Page_1_Custom_PageSize()
        //{
        //    // Arrange 
        //    int totalInDataset = 80;
        //    int pageSize = 33;
        //    await MakeValidWidgets(totalInDataset);
        //    // Act
        //    var response1 = await _sharedTestServerFixture.HttpClient.GetAsync($"/api/v1/widgets?page={1}&pageSize={pageSize}");
        //    var responseObject1 = JObject.Parse(await response1.Content.ReadAsStringAsync());
        //    var widgets1 = responseObject1["result"]["widgets"].ToObject<List<Widget>>();
        //    // Assert
        //    Assert.Equal(pageSize, widgets1.Count);
        //}

        //[Fact]
        //public async Task Get_Widgets_Invalid_Paging_Produces_Error_Json()
        //{
        //    // Arrange
        //    int totalInDataset = 2;
        //    await MakeValidWidgets(totalInDataset);

        //    // Act
        //    var response = await _sharedTestServerFixture.HttpClient.GetAsync($"/api/v1/widgets?page={0}&RecordsPerPage={0}");
        //    Assert.Equal(System.Net.HttpStatusCode.BadRequest, response.StatusCode);
        //}

        //[Fact]
        //public async Task Create_Widget_Returns_201()
        //{
        //    // Arrange
        //    List<Widget> validWidgets = _fakeDataFixture.ValidWidgetProvider.Generate(50).ToList();
        //    List<Task<HttpResponseMessage>> createRequests = new List<Task<HttpResponseMessage>>();
        //    // Act
        //    foreach (var validWidget in validWidgets)
        //    {
        //        var postObject = validWidget.ToAnonymousDto();
        //        createRequests.Add(_sharedTestServerFixture.HttpClient.PostAsync("/api/v1/widgets", postObject.ToJsonStringContent()));
        //    }
        //    await Task.WhenAll(createRequests);
        //    // Assert
        //    for (int i = 0; i < validWidgets.Count; i++)
        //    {
        //        Assert.Equal(System.Net.HttpStatusCode.Created, createRequests[i].Result.StatusCode);
        //        Assert.Contains($"/api/v1/Widgets/", createRequests[i].Result.Headers.Location.ToString());
        //        (await _sharedTestServerFixture.HttpClient.GetAsync(createRequests[i].Result.Headers.Location)).EnsureSuccessStatusCode();
        //        using (var serviceScope = _sharedTestServerFixture.server.Services.CreateScope())//need a using here: https://stackoverflow.com/questions/46063945/cannot-resolve-dbcontext-in-asp-net-core-2-0
        //        {
        //            using (var session = serviceScope.ServiceProvider.GetService<IDocumentStore>().OpenAsyncSession())
        //            {
        //                string ravenVariable = validWidgets[i].WidgetName;
        //                var persistedWidget = await session.Query<Widget>()
        //                    .Where(x => x.WidgetName == ravenVariable)
        //                    .SingleAsync();
        //                Assert.Equal(validWidgets[i].WidgetName, persistedWidget.WidgetName);
        //                Assert.Equal(validWidgets[i].Description, persistedWidget.Description);
        //                Assert.Equal(validWidgets[i].Classification, persistedWidget.Classification);
        //            }
        //        }
        //    }
        //}

        //[Fact]
        //public async Task Create_Widget_Accepts_String_Classification()
        //{
        //    // Arrange
        //    Widget validWidget = _fakeDataFixture.ValidWidgetProvider.Generate(1).Single();
        //    var postObject = validWidget.ToAnonymousDto();
        //    // Act
        //    var response = await _sharedTestServerFixture.HttpClient.PostAsync("/api/v1/widgets", postObject.ToJsonStringContent());
        //    // Assert
        //    Assert.Equal(System.Net.HttpStatusCode.Created, response.StatusCode);
        //    //using (var serviceScope = _sharedTestServerFixture.server.Services.CreateScope())//need a using here: https://stackoverflow.com/questions/46063945/cannot-resolve-dbcontext-in-asp-net-core-2-0
        //    //{
        //    //    var dbContext = serviceScope.ServiceProvider.GetService<WidgetDbContext>();
        //    //    var persistedWidget = dbContext.Widgets.Single(x => x.WidgetName == validWidget.WidgetName);
        //    //    Assert.Equal(validWidget.WidgetName, persistedWidget.WidgetName);
        //    //    Assert.Equal(validWidget.Description, persistedWidget.Description);
        //    //    Assert.Equal(validWidget.Classification, persistedWidget.Classification);
        //    //}
        //    Assert.Contains($"/api/v1/Widgets/", response.Headers.Location.ToString());
        //    var getResponse = await _sharedTestServerFixture.HttpClient.GetAsync(response.Headers.Location);
        //    getResponse.EnsureSuccessStatusCode();
        //}

        //[Fact]
        //public async Task Create_Widget_Invalid_Returns_400()
        //{
        //    // Arrange
        //    Widget invalidWidget = _fakeDataFixture.InvalidWidgetProvider.Generate(1).Single();
        //    var postObject = invalidWidget.ToAnonymousDto();
        //    // Act
        //    var response = await _sharedTestServerFixture.HttpClient.PostAsync("/api/v1/widgets", postObject.ToJsonStringContent());
        //    Assert.Equal(System.Net.HttpStatusCode.BadRequest, response.StatusCode);
        //}

        //[Fact]
        //public async Task Post_Widget_Deprecation_Returns_201()
        //{
        //    // Arrange
        //    await MakeValidWidgets(50);
        //    // Act
        //    var response = await _sharedTestServerFixture.HttpClient.PostAsync($"/api/v1/widgets/5/deprecations", null);
        //    // Assert
        //    Assert.Equal(System.Net.HttpStatusCode.Accepted, response.StatusCode);
        //}
    }
}
