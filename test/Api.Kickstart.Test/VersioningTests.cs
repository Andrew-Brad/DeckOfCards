using System;
using System.Net.Http;
using System.Threading.Tasks;
using System.Linq;
using Api.Kickstart.Test.Fixtures;
using static Api.Kickstart.Test.TestConstants;
using Xunit;

namespace Api.Kickstart.Test
{
    [Collection(SharedServerCollection)]
    public class VersioningTests
    {
        private IntegrationTestServerFixture _sharedTestServerFixture;
        private readonly HttpClient _client;

        public VersioningTests(IntegrationTestServerFixture fixture)//runs once per unit test
        {
            this._sharedTestServerFixture = fixture;
            _client = new HttpClient();
            _client.BaseAddress = HostingUri;
        }

        //I don't particularly like this behaviour but it somewhat depends on the use case and I at least need to have it defined and proven
        [Fact]
        public async Task No_Version_In_Url_Will_Return_Not_Found()
        {
            // Act
            _client.SetAcceptHeaderToJson();
            var response = await _client.GetAsync("/api/widgets");
            Assert.Equal(System.Net.HttpStatusCode.NotFound, response.StatusCode);
            //Assert.True(response.Headers.Single(x => x.Key == "api-supported-versions").Value.Single() == "1");
        }

        //[Fact]
        //public async Task Should_Return_Version2_Header()
        //{
        //    // Act
        //    _client.SetAcceptHeaderToJson();
        //    var response = await _client.GetAsync("/api/2/widgets");
        //    Assert.Equal(System.Net.HttpStatusCode.OK, response.StatusCode);
        //}
    }
}