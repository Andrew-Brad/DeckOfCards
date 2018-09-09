using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Amazon;
using Amazon.SQS;
using Amazon.SQS.Model;
using Polly;

namespace Api.Kickstart.Messaging.Console
{
    /// <summary>
    /// This background service is a stateless service that picks up messages off the queue and does necessary processing.
    /// We dequeue a batch, kick off tasks for all of them, await the group, separate the successes from the failures, then batch delete the
    /// messages corresponding to the succeses.
    /// </summary>
    public class QueuePollingBackgroundService : IHostedService, IDisposable
    {
        private readonly ILogger<QueuePollingBackgroundService> _logger;
        private readonly IConfiguration _config;
        private AmazonSQSClient _sqsClient;
        private CancellationToken _cancellationToken;
        private Task _workLoopTask;

        public QueuePollingBackgroundService(ILogger<QueuePollingBackgroundService> logger, IConfiguration config, AmazonSQSClient sqsClient)
        {
            _logger = logger;
            _config = config;
            _sqsClient = sqsClient;
        }

        public async Task StartAsync(CancellationToken cancellationToken)
        {
            _logger.LogInformation("StartAsync beginning...");
            _cancellationToken = cancellationToken;
            _cancellationToken.Register(() => { _logger.LogInformation("Token cancellation invoked."); });
            //_cancellationSource.Token.Register(() => { _logger.LogInformation("Token cancellation invoked."); });
            _workLoopTask = Task.Factory.StartNew(async () => await PollQueue(), _cancellationToken);
            _logger.LogInformation("StartAsync complete.");
            await Task.CompletedTask;
        }

        private async Task PollQueue()
        {
            _logger.LogDebug("PollQueue() initiated.");

            while (true)
            {
                if (_cancellationToken.IsCancellationRequested) break;
                try
                {
                    // Generic retry policy for out out proc stuff
                    PolicyResult<ReceiveMessageResponse> policyResult = await Policy
                      .Handle<Amazon.Runtime.AmazonServiceException>()
                      //.OrResult<ReceiveMessageResponse>(r => r.HttpStatusCode != System.Net.HttpStatusCode.OK)
                      .WaitAndRetryForeverAsync((x) => TimeSpan.FromSeconds(5), (e, t) => _logger.LogWarning(e, "Retrying SQS poll..."))
                      .ExecuteAndCaptureAsync<ReceiveMessageResponse>(async () =>
                      {
                          ReceiveMessageRequest request = GenerateQueueReceiveRequest();
                          _logger.LogDebug("Polling messages from SQS...");
                          var response = await _sqsClient.ReceiveMessageAsync(request, _cancellationToken);
                          _logger.LogDebug("Polly execute returned the ReceiveMessageRequest.  Status: {status}", response.HttpStatusCode);
                          return response;
                      });
                    //will default(TResult) if policy failed
                    if (policyResult.Outcome != OutcomeType.Successful)
                    {
                        _logger.LogError("Policy did not complete successfully for this poll. Policy Outcome: {policyOutcome}.", policyResult.Outcome);
                        continue;
                    }
                    var receiveMessageResponse = policyResult.Result;

                    if (receiveMessageResponse.HttpStatusCode == System.Net.HttpStatusCode.OK)
                    {
                        _logger.LogInformation("Message polling response came back with {numMessages} messages in queue.", receiveMessageResponse.Messages.Count);
                        if (receiveMessageResponse.Messages.Count == 0) continue;
                        // kick off each piece of work into a background task and only await the whole batch
                        Task<Message>[] bgWorkTasks = StartMessageBackgroundTasks(receiveMessageResponse);
                        await Task.WhenAll(bgWorkTasks);

                        List<Task<Message>> successTasks = bgWorkTasks.Where(x => x.IsCompletedSuccessfully).ToList();
                        List<Task<Message>> failTasks = bgWorkTasks.Where(x => !x.IsCompletedSuccessfully).ToList();
                        _logger.LogInformation("{numSuccessTasks} tasks marked as completing successfully, and {numFailTasks} marked as not completing successfully.", successTasks.Count, failTasks.Count);
                        // todo: what to do here?
                        failTasks.ForEach(x => x.ContinueWith(y => _logger.LogError("Message failures...watdo???")));

                        // Batch delete the successful pieces, let go of the fails
                        DeleteMessageBatchRequest batchDelete = GetBatchMessageRequest(successTasks.Select(x => x.Result).ToList());
                        var batchDeleteResponse = await _sqsClient.DeleteMessageBatchAsync(batchDelete,_cancellationToken);
                        _logger.LogDebug("Message delete response: {statusCode} {@deleteResponseMetadata}", receiveMessageResponse.HttpStatusCode, batchDeleteResponse.ResponseMetadata);
                    }
                    else { _logger.LogError("Http status code on ReceiveMessageResponse did not "); }
                }
                catch (Exception e)
                {
                    _logger.LogError(e, "Error on requesting messages from Queue.");
                }
            }
        }

        private Task<Message>[] StartMessageBackgroundTasks(ReceiveMessageResponse response)
        {
            Task<Message>[] workTasks = new Task<Message>[response.Messages.Count];
            for (int i = 0; i < response.Messages.Count; i++)
            {
                workTasks[i] = ProcessMessage(response.Messages[i]);
            }
            return workTasks;
        }

        private DeleteMessageBatchRequest GetBatchMessageRequest(List<Message> messages)
        {
            var batchDelete = new DeleteMessageBatchRequestEntry[messages.Count];
            for (int i = 0; i < messages.Count; i++)
            {
                batchDelete[i] = new DeleteMessageBatchRequestEntry(messages[i].MessageId, messages[i].ReceiptHandle);
            }
            return new DeleteMessageBatchRequest(_config["AwsSqs:QueueUrl"], batchDelete.ToList());
        }

        private async Task<Message> ProcessMessage(Message message)
        {
            //todo: throw from in here to ensure proper batch deletes
            _logger.LogInformation("Processing message with Id {messageId}...", message.MessageId);
            await Task.Delay(3000,_cancellationToken);
            _logger.LogInformation("Processing complete for message with Id {messageId}...", message.MessageId);
            return await Task.FromResult(message);
        }

        private ReceiveMessageRequest GenerateQueueReceiveRequest()
        {
            var request = new ReceiveMessageRequest
            {
                //AttributeNames = { "SentTimestamp" },
                MaxNumberOfMessages = 10,
                MessageAttributeNames = { "All" },
                QueueUrl = _config["AwsSqs:QueueUrl"],
                WaitTimeSeconds = 10
            };
            return request;
        }

        public async Task StopAsync(CancellationToken cancellationToken)
        {
            _logger.LogInformation("Stopping QueuePollingBackgroundService...");
            //_cancellationToken.Cancel();
            _sqsClient.Dispose();
            //_cancellationToken.Dispose();
            _logger.LogInformation("QueuePollingBackgroundService service stopped.");
            await Task.CompletedTask;
        }

        public void Dispose()
        {
            
        }
    }
}
