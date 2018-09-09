using System;
using System.Collections.Generic;
using System.Text;

namespace Api.Kickstart.Test
{
    public static class TestConstants
    {
        public const string NavigationPathDirectoryToApi = @"..\..\..\..\..\src\Web\ApiKickstart.WebApi";
        public const string SharedServerCollection = "SharedServer";
        public const string DatabaseOutageServerCollection = "DatabaseOutageServer";

        //this works regardless of launch settings and project properties (assuming the port isn't in use by another app)
        public static readonly Uri HostingUri = new Uri("https://localhost:5004");
    }
}
