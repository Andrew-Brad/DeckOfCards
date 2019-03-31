using System;
using System.Collections.Generic;
using System.Net.Sockets;
using System.Reflection;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Amazon.SimpleNotificationService;
using Amazon.SimpleNotificationService.Model;
using Lamar;
using MediatR;
using Newtonsoft.Json;
using NSwag.AspNetCore;
using Polly.Registry;
using Polly;
using Raven.Client.Documents;
using Sieve.Models;
using Sieve.Services;
using DeckOfCards.Domain;
using DeckOfCards.Persistence;
using DeckOfCards.DataModel.JsonContractResolvers;

namespace DeckOfCards.WebApi
{
    public static class StartupExtensions
    {
        public static ServiceRegistry CreateLamarIocContainer()
        {
            var registry = new ServiceRegistry();
            registry.Scan(scanner =>
            {
                // old Structuremap calls fail at runtime, but they compile
                //scanner.IncludeNamespace("ApiKickstart");
                //scanner.LookForRegistries();
                //scanner.AssembliesFromApplicationBaseDirectory();
                //scanner.TheCallingAssembly();
                scanner.WithDefaultConventions();

                //scanner.Assembly(Assembly.Load(nameof(DeckOfCards.QueryHandlers)));
                //scanner.Assembly(Assembly.Load(nameof(DeckOfCards.CommandHandlers)));
                scanner.Assembly(Assembly.Load("DeckOfCards.QueryHandlers")); // todo: improve assembly targeting logic
                scanner.Assembly(Assembly.Load("DeckOfCards.CommandHandlers"));
                //scanner.AssemblyContainingType<CardTemplateQueryHandler>(); 
                //scanner.AssemblyContainingType<NewDeckOfCardsCommandHandler>(); // todo: improve assembly targeting logic

                // auto register the open generics for our handler classes - https://github.com/wooderz/MediatR/wiki
                scanner.ConnectImplementationsToTypesClosing(typeof(IRequestHandler<>)); // Handlers with no response
                scanner.ConnectImplementationsToTypesClosing(typeof(IRequestHandler<,>)); // Handlers with a response
                scanner.ConnectImplementationsToTypesClosing(typeof(INotificationHandler<>));
            });

            // Mediatr works best when configured directly with your IoC container - https://github.com/jbogard/MediatR/wiki
            registry.For<IMediator>().Use<Mediator>().Scoped();
            registry.For<ServiceFactory>().Use(ctx => ctx.GetInstance);
            return registry;
        }

        public static IApplicationBuilder AddCustomSwagger(this IApplicationBuilder app)
        {
            var config = app.ApplicationServices.GetRequiredService<IConfiguration>();
            var env = app.ApplicationServices.GetRequiredService<IHostingEnvironment>();
            var logger = app.ApplicationServices.GetRequiredService<ILogger<Startup>>();
            logger.LogTrace("Beginning DI registration for Swagger...");
            //TODO: Many ways to do this now with 2.0 - https://github.com/RSuter/NSwag/blob/master/src/NSwag.Sample.NETCore20/Startup.cs
            if (bool.Parse(config["EnableSwagger"]))
            {
                app.UseSwagger();
                app.UseSwaggerUi3();

                //app.UseSwaggerUi(typeof(Startup).GetTypeInfo().Assembly, s =>
                //{
                //    s.GeneratorSettings.DefaultPropertyNameHandling = NJsonSchema.PropertyNameHandling.CamelCase;
                //    s.GeneratorSettings.AddMissingPathParameters = true;//needed due to known issues with [HttpGet] attributes with default routes/controller route attributes
                //    s.GeneratorSettings.Title = $"Cards Api - {env.EnvironmentName}";
                //    s.GeneratorSettings.Description = "The Cards Api enables you to progammatically create and manage decks of cards.";
                //});
            }
            logger.LogTrace("Swagger configuration complete.");
            return app;
        }

        /// <summary>
        /// Configures simple notification service into your application's IoC container.
        /// </summary>
        /// <param name="services"></param>
        /// <returns></returns>
        public static IServiceCollection AddAwsSns(this IServiceCollection services)
        {
            IConfiguration config = services.BuildServiceProvider().GetRequiredService<IConfiguration>();
            ILogger logger = services.BuildServiceProvider().GetRequiredService<ILogger<Startup>>();
            //Aws:
            logger.LogDebug("Beginning DI registration of AWS SNS objects...");
            string snsTopicArn = config["AwsSns:TopicArn"];
            string accessKeyId = config["AwsSns:AccessKeyId"];
            string secretkey = config["AwsSns:SecretKey"];
            string region = config["AwsSns:Region"];
            var snsConfig = new AmazonSimpleNotificationServiceConfig();
            snsConfig.RegionEndpoint = Amazon.RegionEndpoint.GetBySystemName(region);
            var snsclient = new AmazonSimpleNotificationServiceClient(accessKeyId,secretkey,snsConfig);
            services.AddSingleton(snsConfig);
            services.AddSingleton(snsclient);
            return services;
        }

        /// <summary>
        /// Configures resilience and fault tolerance policies for global application use.
        /// </summary>
        /// <param name="services"></param>
        /// <returns></returns>
        public static IServiceCollection AddResiliencePolicies(this IServiceCollection services)
        {
            IConfiguration config = services.BuildServiceProvider().GetRequiredService<IConfiguration>();
            ILogger logger = services.BuildServiceProvider().GetRequiredService<ILogger<Startup>>();
            logger.LogTrace("Configuring Polly policies...");

            // Polly
            PolicyRegistry registry = new PolicyRegistry();

            // Db Commands
            IAsyncPolicy<int> databaseCommandTransientRetryPolicy = Policy<int>
                .Handle<SocketException>()
                //.Or<System.Net.Sockets.SocketException>()
                .WaitAndRetryAsync(3, x => TimeSpan.FromSeconds(1),
                    onRetryAsync: (e, t, i, c) =>
                    {
                        // hidden case here where ILogger will report this retry under the wrong Source (this namespace)
                        logger.LogDebug("Transient database error, retry #" + i + " attempting...");
                        return Task.CompletedTask;
                    });
            registry.Add("DbCommand", databaseCommandTransientRetryPolicy);

            // Db Queries
            IAsyncPolicy<int> databaseQueryTransientRetryPolicy = Policy<int>
                .Handle<SocketException>()
                //.Or<System.Net.Sockets.SocketException>()
                .WaitAndRetryAsync(10, x => TimeSpan.FromSeconds(1),
                    onRetryAsync: (e, t, i, c) =>
                    {
                        logger.LogInformation("Transient database error, retry #" + i + " attempting...");
                        return Task.CompletedTask;
                    });
            registry.Add("DbQuery", databaseQueryTransientRetryPolicy);

            // Cloud Messaging
            IAsyncPolicy<PublishResponse> cloudMessagePublishRetryPolicy = Policy<PublishResponse>
                .Handle<Amazon.Runtime.AmazonServiceException>()
                .WaitAndRetryForeverAsync((x) => TimeSpan.FromSeconds(10),
                    onRetryAsync: (e, t, i) =>
                    {
                        logger.LogDebug("Transient database error, retry #" + i + " attempting...");
                        return Task.CompletedTask;
                    });
            registry.Add("CloudMessagePublish", cloudMessagePublishRetryPolicy);

            services.AddSingleton(registry);
            logger.LogTrace("Polly configuration and registration complete.");
            return services;
        }

        /// <summary>
        /// Configures sorting, filtering, and paging objects.
        /// </summary>
        /// <param name="services"></param>
        /// <returns></returns>
        public static IServiceCollection AddSortFilterPaging(this IServiceCollection services)
        {
            IConfiguration config = services.BuildServiceProvider().GetRequiredService<IConfiguration>();
            ILogger logger = services.BuildServiceProvider().GetRequiredService<ILogger<Startup>>();
            // Sieve:
            services.AddScoped<ISieveProcessor,SieveProcessor>();
            services.Configure<SieveOptions>(config.GetSection("Sieve"));
            return services;
        }

        /// <summary>
        /// Adds a singleton <see cref="IDocumentStore"/>
        /// </summary>
        /// <param name="services"></param>
        /// <returns></returns>
        public static IServiceCollection AddRavenDb(this IServiceCollection services)
        {
            IConfiguration config = services.BuildServiceProvider().GetRequiredService<IConfiguration>();
            ILogger logger = services.BuildServiceProvider().GetRequiredService<ILogger<Startup>>();            
            logger.LogDebug("Beginning DI registration of RavenDB DocumentStore...");
            DocumentStore store = InitializeRavenDbDocumentStore(config);
            
            // "Because the document store is a heavyweight object, there should only be one instance created per application (singleton)." - the docs
            services.AddSingleton<IDocumentStore>(store);
            return services;
        }

        public const string ConfigKeyRavenDbUrl = "RavenDb:Urls";
        public const string ConfigKeyRavenDbDatabase = "RavenDb:Database";
        public static DocumentStore InitializeRavenDbDocumentStore(IConfiguration config)
        {
            DocumentStore store = new DocumentStore()
            {
                Urls = config.GetSection(ConfigKeyRavenDbUrl).Get<string[]>(),
                Database = config[ConfigKeyRavenDbDatabase]
            };

            store.Conventions.CustomizeJsonSerializer = AddCustomConverters;
            store.Conventions.RegisterAsyncIdConvention<CardTemplate>(IdConventions.CardTemplateIdStrategy);
            // All customizations need to be set before DocumentStore.Initialize() is called.
            // https://ravendb.net/docs/article-page/4.0/csharp/client-api/configuration/conventions
            store.Initialize();
            return store;
        }

        /// <summary>
        /// It is preferred to do Json serializer customization and configuration here, as opposed
        /// to tagging a bunch of Json attributes in your domain model.
        /// </summary>
        /// <param name="ravenJsonSerializer"></param>
        private static void AddCustomConverters(JsonSerializer ravenJsonSerializer)
        {
            ravenJsonSerializer.Converters.Add(new SuitsEnumerationConverter());
            ravenJsonSerializer.Converters.Add(new RanksEnumerationConverter());
            ravenJsonSerializer.Converters.Add(new DeckConverter());
            ravenJsonSerializer.Converters.Add(new PlayingCardConverter()); 
        }
    }
}
