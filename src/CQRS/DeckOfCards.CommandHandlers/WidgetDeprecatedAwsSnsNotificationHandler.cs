using MediatR;
using System;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using Polly;
using System.Threading;
using Amazon.SimpleNotificationService;
using Microsoft.Extensions.Configuration;
using Amazon.SimpleNotificationService.Model;
using ApiKickstart.DomainEvents;
using Polly.Registry;
using ApiKickstart.CQRS;

namespace ApiKickstart.QueryHandlers
{
    public class WidgetDeprecatedAwsSnsNotificationHandler : INotificationHandler<WidgetDeprecatedV1Event>
    {
        private readonly AmazonSimpleNotificationServiceClient _snsClient;
        private readonly IConfiguration _config;
        private readonly ILogger<WidgetDeprecatedAwsSnsNotificationHandler> _logger;
        private readonly IReadOnlyPolicyRegistry<string> _policyRegistry;

        public WidgetDeprecatedAwsSnsNotificationHandler(AmazonSimpleNotificationServiceClient snsClient, IConfiguration config,
            ILogger<WidgetDeprecatedAwsSnsNotificationHandler> logger, IReadOnlyPolicyRegistry<string> registry)
        {
            _snsClient = snsClient;
            _config = config;
            _logger = logger;
            _policyRegistry = registry;
        }

        public async Task Handle(WidgetDeprecatedV1Event notification, CancellationToken cancellationToken)
        {
            try
            {
                var policy = _policyRegistry.Get<IAsyncPolicy<PublishResponse>>("CloudMessagePublish");
                var policyResult = await policy
                    .ExecuteAndCaptureAsync(async () =>
                  {
                      var publishRequest = FromEvent(notification);
                      return await _snsClient.PublishAsync(publishRequest);
                  });
                //will default(TResult) if policy failed
                _logger.LogTrace("Policy completed for widget deprecation. Policy Outcome: {policyOutcome}. MessageId: {messageId}", policyResult.Outcome, policyResult.Result?.MessageId);
            }
            catch (Exception e)
            {
                _logger.LogError("Notification handler failed to publish domain event to SNS. Event: {@event}", e, notification);
            }
        }

        // temporary shortcut method
        public PublishRequest FromEvent(IEvent broadcastEvent)
        {
            PublishRequest publishRequest = new PublishRequest();
            publishRequest.TopicArn = (_config["AwsSns:TopicArn"]);
            publishRequest.Subject = broadcastEvent.EventName;
            publishRequest.Message = JsonConvert.SerializeObject(broadcastEvent);
            //todo: tag version here
            //publishRequest.MessageAttributes
            return publishRequest;
        }
    }
}
