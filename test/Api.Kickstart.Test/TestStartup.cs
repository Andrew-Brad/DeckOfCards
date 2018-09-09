using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using ApiKickstart.WebApi;

namespace Api.Kickstart.Test
{
    /// <summary>
    /// This class currently isn't used (conflicts with runtime IoC DLL scanning), but is
    /// a good reference for Startup customization, especially since Asp.Net Core allows
    /// the environment function name convention (ex: <see cref="ConfigureDevelopmentServices(IServiceCollection)"/>
    /// </summary>
    public class TestStartup : Startup
    {
        public TestStartup(IConfiguration configuration, ILogger<Startup> logger) : base(configuration, logger)
        {
            
        }

        public void ConfigureDevelopmentServices(IServiceCollection services)
        {
            // First, take all the base registrations
            base.ConfigureServices(services);

            // Then customize DI for unit testing requirements

        }
    }
}
