using System;
using System.Net.Http;
using System.Threading.Tasks;
using Newtonsoft.Json.Linq;
using Xunit;
using DeckOfCards.Test.Fixtures;
using static DeckOfCards.Test.TestConstants;

namespace DeckOfCards.Test
{
    [Collection(SharedServerCollection)]
    public class AcceptHeaderTests
    {
        private readonly IntegrationTestServerFixture _sharedTestServerFixture;

        public AcceptHeaderTests(IntegrationTestServerFixture fixture)//runs once per unit test
        {
            this._sharedTestServerFixture = fixture;
        }

        [Fact]
        public async Task Json_Accept_Header_Should_Return_Json()
        {
            //Arrange:
            var client = new HttpClient();
            client.BaseAddress = HostingUri;
            // Act
            var response = await client.GetAsync("/api/v1/health");
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
            var response = await client.GetAsync("/api/v1/health");
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
            var response = await client.GetAsync("/api/v1/health");
            response.EnsureSuccessStatusCode();
            var responseString = await response.Content.ReadAsStringAsync();
            // Assert
            JObject.Parse(responseString);
        }

        //[Fact]
        //public async Task Api_Should_Return_Xml_For_Xml_Accept_Header()
        //{
        //    //Arrange
        //    var client = new HttpClient();
        //    client.BaseAddress = HostingUri;
        //    // Act
        //    client.SetAcceptHeaderToXml();
        //    var response = await client.GetAsync("/api/v1/health");
        //    response.EnsureSuccessStatusCode();
        //    var responseString = await response.Content.ReadAsStringAsync();
        //    var xmlDoc = XDocument.Parse(responseString);
        //}

        //[Theory]
        //[InlineData(-1)]
        //[InlineData(0)]
        //[InlineData(1)]
        //public void ReturnFalseGivenValuesLessThan2(int value)
        //{
        //    var result = 1 / value;
        //    Assert.True(result > 0);
        //}
    }
}
