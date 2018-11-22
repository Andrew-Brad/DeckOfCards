using System;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using MediatR;
using AB.Extensions;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Versioning;
using AutoMapper;
using AB.Middleware.HttpRequestLogging;
using Lamar;

namespace DeckOfCards.WebApi
{
    public class Startup
    {
        #region Provided By Asp.Net DI
        private readonly ILogger<Startup> _logger;
        private readonly IConfiguration _configuration;
        private readonly IHostingEnvironment _environment;
        #endregion

        public Startup(IConfiguration configuration, ILogger<Startup> logger, IHostingEnvironment hostingEnvironment)
        {
            _configuration = configuration;
            _logger = logger;
            _environment = hostingEnvironment;
        }

        // This method gets called by the runtime. Use this method to add services to the container.
        public void ConfigureServices(IServiceCollection services)
        {
            _logger.LogDebug("ConfigureServices method entered.");

            // Https
            //services.AddHsts(x => x.IncludeSubDomains = true);
            //services.AddHttpsRedirection(options =>
            //{
            //    //options.RedirectStatusCode = StatusCodes.Status301MovedPermanently;
            //    //options.HttpsPort = 5001;
            //});

            // Add framework services.
            var mvcBuilder = services.AddMvc();
            mvcBuilder.AddMvcOptions(options => options.OutputFormatters.Add(new Microsoft.AspNetCore.Mvc.Formatters.XmlDataContractSerializerOutputFormatter(new System.Xml.XmlWriterSettings() { NamespaceHandling = System.Xml.NamespaceHandling.OmitDuplicates, Async = true, OmitXmlDeclaration = false })));//TODO: configure removal of namespacing in the resulting xml
            mvcBuilder.AddMvcOptions(options => options.ReturnHttpNotAcceptable = true);
            mvcBuilder.SetCompatibilityVersion(CompatibilityVersion.Version_2_1); // required for certain features like [ApiController]
            //var jsonFormatter = mvcBuilder.

            // In memory cache
            services.AddMemoryCache();

            // Mediatr + handlers
            services.AddMediatR(typeof(Startup).Assembly);

            // Mapping:
            services.AddAutoMapper();

            // Resilience Policies:
            services.AddResiliencePolicies();

            // Data Access:
            services.AddRavenDb();

            // Versioning:           
            services.AddApiVersioning(options =>
            {
                options.ReportApiVersions = true;
                options.AssumeDefaultVersionWhenUnspecified = false;
                options.DefaultApiVersion = new ApiVersion(1, 0);
                options.ApiVersionReader = new UrlSegmentApiVersionReader();
                options.ApiVersionSelector = new CurrentImplementationApiVersionSelector(options);
            });

            //Messaging:
            services.AddAwsSns();

            // Generic sorting/filtering/paging
            services.AddSortFilterPaging();
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IApplicationLifetime appLifetime)
        {
            _logger.LogDebug("Configure method entered.");

            // order matters with UseMiddleware()!
            app.UseMiddleware<HttpRequestSummaryLoggingMiddleware>();

            if (_environment.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }
            else
            {
                app.UseHsts();
            }

            app.UseHttpsRedirection();
            app.UseMvc();

            // Enable middleware to serve swagger-ui assets (HTML, JS, CSS etc.) (NOTE: Doesn't work with versioning...https://github.com/NSwag/NSwag/issues/655)
            app.AddCustomSwagger();

            _logger.LogInformation("Startup initialization completed for {environment}.", _environment.EnvironmentName);
        }

        /// <summary>
        /// Invoked by 3rd Party dependency injection to enable advanced IoC features
        /// Roadblocking issue here: https://github.com/JasperFx/lamar/issues/67
        /// Potentially some nasty interactions with Mediatr and/or Automapper registrations? I believe something is
        /// causing a circular reference in the IoC container (even though we've enabled validation for this in Development)
        /// </summary>
        /// <param name="services"></param>
        public void ConfigureContainer(ServiceRegistry services)
        {
            // Register stuff in container
            services.Scan(scanner =>
            {
                //scanner.IncludeNamespace("ApiKickstart");
                scanner.WithDefaultConventions();
                scanner.LookForRegistries();
                scanner.AssembliesFromApplicationBaseDirectory();

                // auto register the open generics for our handler classes:
                scanner.ConnectImplementationsToTypesClosing(typeof(IRequestHandler<,>)); // http://structuremap.github.io/generics/#sec1
                scanner.ConnectImplementationsToTypesClosing(typeof(INotificationHandler<>)); // also: https://github.com/jbogard/MediatR/wiki
            });
        }
    }
}
