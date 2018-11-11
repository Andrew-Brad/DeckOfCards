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
using Raven.Client.Documents.Session;
using System.Collections.Generic;

namespace DeckOfCards.QueryHandlers
{
    public class NewDeckOfCardsCommandHandler : IRequestHandler<NewDeckOfCardsCommand, NewDeckOfCardsCommandResult>
    {
        private readonly ILogger<NewDeckOfCardsCommandHandler> _logger;
        private readonly IDocumentStore _db;
        private readonly IMapper _mapper;
        private readonly IReadOnlyPolicyRegistry<string> _policyRegistry;

        public NewDeckOfCardsCommandHandler(ILogger<NewDeckOfCardsCommandHandler> logger, IDocumentStore db, IMapper mapper, IReadOnlyPolicyRegistry<string> registry)
        {
            _logger = logger;
            _db = db;
            _mapper = mapper;
            _policyRegistry = registry;
        }

        public async Task<NewDeckOfCardsCommandResult> Handle(NewDeckOfCardsCommand command, CancellationToken cancellationToken)
        {
            var commandResult = new NewDeckOfCardsCommandResult();
            try
            {
                IAsyncPolicy<int> policy = _policyRegistry.Get<IAsyncPolicy<int>>("DbCommand");
                var policyResult = await policy
                    .ExecuteAndCaptureAsync(async () =>
                    {
                        using (var session = _db.OpenAsyncSession())
                        {
                            // pull our Card Templates - this ultimately defines what 'standard' cards really are
                            QueryStatistics queryStats;
                            var allDbWidgetsQuery = session.Query<CardTemplate>()
                                .Statistics(out queryStats);
                            List<CardTemplate> cardTemplates = await allDbWidgetsQuery.ToListAsync();

                            // call into the Deck Aggregate to create us a deck entity object
                            var deck = Deck.Standard52CardDeck(cardTemplates);

                            // persist
                            await session.StoreAsync(deck);
                            await session.SaveChangesAsync();
                            commandResult.Deck = deck;
                            return 1;
                        }
                    });

                if (policyResult.Outcome == OutcomeType.Failure) return ServiceUnavailableCommandResult();
                else commandResult.ResultStatus = CQRS.CommandResultStatus.SuccessfullyProcessed;
            }
            catch (Exception e)
            {
                _logger.LogError(e, "Exception caught and logged.");
                commandResult.ResultStatus = CQRS.CommandResultStatus.CriticalError;
            }
            finally
            {
                // - will remove when logging and validation pipelines are completed in the Dispatch layer
            }
            return commandResult;
        }

        // todo: make this method generic and prevent copy paste
        private NewDeckOfCardsCommandResult ServiceUnavailableCommandResult()
        {
            _logger.LogError("Policy outcome was failure."); // warning?
             return new NewDeckOfCardsCommandResult() { ResultStatus = CQRS.CommandResultStatus.ServiceUnavailable };
        }
    }
}
