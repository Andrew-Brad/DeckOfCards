using System;
using System.Net.Http;
using System.Threading.Tasks;
using System.Linq;
using Xunit;
using static DeckOfCards.Test.TestConstants;
using DeckOfCards.Test.Fixtures;

namespace DeckOfCards.Kickstart.Test
{
    [Collection(SharedServerCollection)]
    public class LocationHeaderTest// : IClassFixture<SharedTestServerFixture>
    {
        private IntegrationTestServerFixture _sharedTestServerFixture;
        private readonly HttpClient _client;

        public LocationHeaderTest(IntegrationTestServerFixture fixture)//runs once per unit test
        {
            this._sharedTestServerFixture = fixture;
            _client = new HttpClient();
            _client.BaseAddress = HostingUri;
        }

        //[Fact]
        //public async Task Should_Return_Accepted_For_Resource_Created()
        //{
        //    // Act
        //    _client.SetAcceptHeaderToJson();
        //    var response = await _client.PostAsync("/api/v1/widgets/2/deprecations",null);
        //    Assert.Equal(System.Net.HttpStatusCode.Accepted, response.StatusCode);
        //    Assert.EndsWith("api/v1/widgets/2",response.Headers.Location.OriginalString, StringComparison.CurrentCultureIgnoreCase);
        //}
    }
}