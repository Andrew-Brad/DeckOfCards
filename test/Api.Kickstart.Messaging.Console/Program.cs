using System.Threading.Tasks;
using Amazon;
using Amazon.SQS;
using DeckOfCards.Kickstart.Messaging.Console;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Serilog;

namespace DeckOfCards.Kickstart.Messaging
{
    class Program
    {
        public static async Task Main(string[] args)
        {
            var builder = new HostBuilder()
                .ConfigureAppConfiguration((hostingContext, config) =>
                {
                    config.AddJsonFile("appsettings.json", optional: true);
                    config.AddEnvironmentVariables();
                    config.AddUserSecrets("aspnet-ApiKickstart-9c9e22fb-27d7-4783-99d8-6d7253dbf01b");
                    if (args != null)
                    {
                        config.AddCommandLine(args);
                    }                    
                })
                .ConfigureServices((hostContext, services) =>
                {
                    var sqsClient = new AmazonSQSClient(
                        hostContext.Configuration["AwsSns:AccessKeyId"],
                        hostContext.Configuration["AwsSns:SecretKey"],
                        RegionEndpoint.GetBySystemName(hostContext.Configuration["AwsSns:Region"]));
                    services.AddSingleton(sqsClient);
                    services.AddSingleton<IHostedService, QueuePollingBackgroundService>();
                })
                .ConfigureLogging((hostingContext, logging) => 
                {
                    //logging.AddConfiguration(hostingContext.Configuration.GetSection("Logging"));
                    //logging.AddConsole();
                    logging.SetMinimumLevel(LogLevel.Trace);
                    var loggerConfig = new LoggerConfiguration();
                    loggerConfig
                        .MinimumLevel.Verbose()
                        .Enrich.WithMachineName()
                        .Enrich.WithProcessId()
                        .Enrich.WithThreadId()
                        .WriteTo.Console(outputTemplate: "[{MachineName} - ThreadId:{ThreadId}] [{Timestamp:HH:mm:ss} {Level:u4}] {Message:lj}{NewLine}{Exception}");
                    logging.AddSerilog(loggerConfig.CreateLogger());
                })
                ;

            await builder.RunConsoleAsync();
        }
    }
}
