using System;
using System.Collections.Generic;
using System.IO;
using Microsoft.AspNetCore.Hosting;
using Serilog;
using Microsoft.Extensions.Configuration;
using AB.Extensions;
using StructureMap.AspNetCore;

namespace DeckOfCards.WebApi
{
    public class Program
    {
        public static int Main(string[] args)
        {
            try
            {
                Log.Information("Starting web host.");
                IWebHostBuilder hostBuilder = CreateWebHostBuilder(args);
                IWebHost host = hostBuilder.Build();
                host.Run();
                return 0;
            }
            catch (Exception ex)
            {
                Log.Fatal(ex, "Host terminated unexpectedly.");
                return 1;
            }
            finally
            {
                Log.CloseAndFlush();
            }
        }

        /* 
         * Customized WebHostBuilder for big boy projects
         * For reference: https://joonasw.net/view/aspnet-core-2-configuration-changes)
         * EF Core will use reflection to pick up a method that returns IWebHostBuilder
         */
        public static IWebHostBuilder CreateWebHostBuilder(string[] args)
        {
            return new WebHostBuilder()
                .UseKestrel(x => x.AddServerHeader = false) // more stuff for kestrel options and HTTPS: https://docs.microsoft.com/en-us/aspnet/core/fundamentals/servers/kestrel?view=aspnetcore-2.1&tabs=aspnetcore2x#endpoint-configuration
                .UseContentRoot(Directory.GetCurrentDirectory())
                .ConfigureAppConfiguration((hostingContext, config) =>
                {
                    //you can clear, add, or override some configuration here:
                    IHostingEnvironment env = hostingContext.HostingEnvironment;
                    config                    
                        .AddJsonFile($"Configuration/appsettings.json", optional: false, reloadOnChange: true)
                        .AddJsonFile($"Configuration/appsettings.{env.EnvironmentName}.json", optional: false, reloadOnChange: true)
                        .AddJsonFile($"Configuration/aws-sns-settings.json", optional: false, reloadOnChange: true)
                        .AddUserSecrets("aspnet-ApiKickstart-9c9e22fb-27d7-4783-99d8-6d7253dbf01b")
                        .AddEnvironmentVariables();

                    // Configuration keys are "last in wins"
                    if (env.IsDevelopment())
                    {
                        var appAssembly = System.Reflection.Assembly.Load(new System.Reflection.AssemblyName(env.ApplicationName));
                        if (appAssembly != null) config.AddUserSecrets(appAssembly, optional: true);
                    }

                    if (args != null) config.AddCommandLine(args);
                })
                .ConfigureLogging((hostingContext, logging) =>
                {
                    //https://nblumhardt.com/2017/08/use-serilog/
                    Log.Logger = ConfigureSerilog(hostingContext.Configuration, hostingContext.HostingEnvironment).CreateLogger();
                })
                .UseIISIntegration()
                .UseDefaultServiceProvider((context, options) =>
                {
                    options.ValidateScopes = context.HostingEnvironment.IsDevelopment();
                })                
                .UseSerilog()
                .UseStructureMap()                
                .UseStartup<Startup>();
        }

        public static LoggerConfiguration ConfigureSerilog(IConfiguration config, IHostingEnvironment env)
        {
            var loggingConfig = config.GetSection("Logging");
            var serilogDbSinkConfig = loggingConfig.GetSection("Api:DatabaseLogger");
            LoggerConfiguration logConfig = new LoggerConfiguration()
                .MinimumLevel.Verbose()//best practice is to focus your log levels per sink and leave this verbose
                .MinimumLevel.Override("Microsoft", config["Logging:MasterOverrides:Microsoft"].ToEnumTypeOf<Serilog.Events.LogEventLevel>())
                .MinimumLevel.Override("System", config["Logging:MasterOverrides:Microsoft"].ToEnumTypeOf<Serilog.Events.LogEventLevel>())
                .Enrich.FromLogContext()
                //.Enrich.WithThreadId()
                // Process enricher:
                .Enrich.WithProcessId()
                // Enviro enrichers:
                .Enrich.WithMachineName()
                .Enrich.WithEnvironmentUserName()

                // Configure sinks:
                .WriteTo.Logger(lc => lc
                    .WriteTo.Debug())
                .WriteTo.Logger(lc => lc
                    .WriteTo.LiterateConsole()) //a pretty print version over the traditional Sinks.Console
                .WriteTo.Async(lc => lc
                    .RollingFile(env.ContentRootPath + "/" + config["Logging:Api:RollingFile:Folder"] + "/" + config["Logging:Api:RollingFile:FileName"],
                        config["Logging:Api:RollingFile:LogLevel"].ToEnumTypeOf<Serilog.Events.LogEventLevel>(),
                        fileSizeLimitBytes: long.Parse(config["Logging:Api:RollingFile:FileSizeLimitBytes"]),
                        retainedFileCountLimit: int.Parse(config["Logging:Api:RollingFile:RetainedFileCountLimit"]),
                        shared: true))
                // can keep adding sinks - close parentheses per sink     

                // todo: long term logging solution/sink for something like dynamo or elastic search for a uniform logging/data extraction solution


                ; // end logger sink configuration

            if (bool.Parse(config["Logging:EnableSelfLog"]))
            {
                var file = File.CreateText("SelfLogging.txt");
                Serilog.Debugging.SelfLog.Enable(TextWriter.Synchronized(file)); 
            }
            return logConfig;
        }
    }
}
