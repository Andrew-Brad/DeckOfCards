using System;
using System.Collections.Generic;
using System.Text;

namespace DeckOfCards.Test
{
    public static class TestConstants
    {
        public const string NavigationPathDirectoryToApi = @"..\..\..\..\..\src\Web\DeckOfCards.WebApi";
        public const string SharedServerCollection = "SharedServer";
        public const string DatabaseOutageServerCollection = "DatabaseOutageServer";
        public const string DataProviderCollection = "FakeData";

        //this works regardless of launch settings and project properties (assuming the port isn't in use by another app)
        public static readonly Uri HostingUri = new Uri("https://localhost:5004");
    }
}
