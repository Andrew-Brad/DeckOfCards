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
using DeckOfCards.Commands;
using DeckOfCards.CommandResults;

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

        // Take in Lamar's ServiceRegistry instead of IServiceCollection
        // as your argument, but fear not, it implements IServiceCollection as well
        public void ConfigureContainer(ServiceRegistry services)
        {
            _logger.LogDebug("ConfigureServices method entered.");

            //Https
            //services.AddHsts(x => x.IncludeSubDomains = true);
            //services.AddHttpsRedirection(options =>
            //{
            //    //options.RedirectStatusCode = StatusCodes.Status301MovedPermanently;
            //    //options.HttpsPort = 5001;
            //});

            //Add framework services.
            var mvcBuilder = services.AddMvc();
            mvcBuilder.AddMvcOptions(options => options.OutputFormatters.Add(new Microsoft.AspNetCore.Mvc.Formatters.XmlDataContractSerializerOutputFormatter(new System.Xml.XmlWriterSettings() { NamespaceHandling = System.Xml.NamespaceHandling.OmitDuplicates, Async = true, OmitXmlDeclaration = false })));//TODO: configure removal of namespacing in the resulting xml
            mvcBuilder.AddMvcOptions(options => options.ReturnHttpNotAcceptable = true);
            mvcBuilder.SetCompatibilityVersion(CompatibilityVersion.Version_2_1); // required for certain features like [ApiController]
            //var jsonFormatter = mvcBuilder.


            // Swagger
            services.AddSwaggerDocument();

            // In memory cache
            services.AddMemoryCache();

            // Resilience Policies:
            services.AddResiliencePolicies();

            // Data Access:
            services.AddRavenDb();

            //Versioning:           
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
    }
}
