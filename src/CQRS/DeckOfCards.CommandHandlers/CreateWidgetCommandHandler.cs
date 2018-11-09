using System;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using DeckOfCards.Commands;
using DeckOfCards.CommandResults;
using DeckOfCards.Domain;
using MediatR;
using Polly;
using AutoMapper;
using Polly.Registry;
using Raven.Client.Documents;

namespace DeckOfCards.QueryHandlers
{
    public class CreateWidgetCommandHandler : IRequestHandler<CreateWidgetCommand, CreateWidgetCommandResult>
    {
        private readonly ILogger<CreateWidgetCommandHandler> _logger;
        private readonly IDocumentStore _widgetDb;
        private readonly IMapper _mapper;
        private readonly IReadOnlyPolicyRegistry<string> _policyRegistry;

        public CreateWidgetCommandHandler(ILogger<CreateWidgetCommandHandler> logger, IDocumentStore db, IMapper mapper, IReadOnlyPolicyRegistry<string> registry)
        {
            _logger = logger;
            _widgetDb = db;
            _mapper = mapper;
            _policyRegistry = registry;
        }

        public async Task<CreateWidgetCommandResult> Handle(CreateWidgetCommand command, CancellationToken cancellationToken)
        {
            var commandResult = new CreateWidgetCommandResult();
            try
            {
                var newWidget = _mapper.Map<CardTemplate>(command);
                //newWidget.Id = Guid.NewGuid().ToString();
                var policy = _policyRegistry.Get<IAsyncPolicy<int>>("DbCommand");
                var policyResult = await policy
                    .ExecuteAndCaptureAsync(async () =>
                  {
                      using (var session = _widgetDb.OpenAsyncSession())
                      {
                          await session.StoreAsync(newWidget);
                          await session.SaveChangesAsync();
                          return 1;
                      }
                  });
                if (policyResult.Outcome == OutcomeType.Failure) return ServiceUnavailableCommandResult();

                commandResult.Widget = CardTemplate.NewCard(RanksEnumeration.Ace, SuitsEnumeration.Spades); //todo - automapper
                
                commandResult.ResultStatus = CQRS.CommandResultStatus.SuccessfullyProcessed;
                _logger.LogTrace("{result} has completed. {widget} returned.", nameof(CreateWidgetCommandResult), commandResult.Widget);
            }
            catch (Exception e)
            {
                _logger.LogError(new EventId(), e, "Oops! Ran into an error!");
                commandResult.ResultStatus = CQRS.CommandResultStatus.CriticalError;
            }
            finally
            {
                //log?

            }
            return commandResult;
        }

        private CreateWidgetCommandResult ServiceUnavailableCommandResult()
        {
            _logger.LogError("Policy outcome was failure."); // warning?
             return new CreateWidgetCommandResult() { ResultStatus = CQRS.CommandResultStatus.ServiceUnavailable };
        }
    }
}
