using System;
using System.Threading.Tasks;
using System.Threading;
using Microsoft.Extensions.Logging;
using DeckOfCards.CQRS;
using DeckOfCards.Domain;
using DeckOfCards.QueryResults;
using DeckOfCards.Queries;
using AutoMapper;
using MediatR;
using Raven.Client.Documents;

namespace DeckOfCards.QueryHandlers
{
    public class DeckOfCardsQueryHandler : IRequestHandler<DeckOfCardsQuery, NewDeckOfCardsQueryResult>
    {
        private readonly ILogger<DeckOfCardsQueryHandler> _logger;
        private readonly IDocumentStore _db;
        private readonly IMapper _mapper;

        public DeckOfCardsQueryHandler(ILogger<DeckOfCardsQueryHandler> logger, IDocumentStore docStore, IMapper mapper)
        {
            _logger = logger;
            _db = docStore;
            _mapper = mapper;
        }

        public async Task<NewDeckOfCardsQueryResult> Handle(DeckOfCardsQuery query, CancellationToken cancellationToken)
        {
            var queryResult = new NewDeckOfCardsQueryResult();
            try
            {
                using (var session = _db.OpenAsyncSession())
                {
                    var deck = await session
                        .LoadAsync<Deck>(query.DeckId);



                    //,
                    //i => i.IncludeDocuments<CardTemplate>(x => x.ShowCards().Select(y => y.Template.Rank.Name)));
                    //string.Format("cards/{0}/{1}", card.Rank.Name, card.Suit)));
                    queryResult.Deck = deck;
                }
                
                queryResult.ResultStatus = queryResult.Deck != null ? QueryResultStatus.SuccessfullyProcessed : QueryResultStatus.NoResultData;//potential to wrap this into its own generic extension method looking at Result<T>
            }
            catch (Exception e)
            {
                _logger.LogError(e, "Error occurred on query.");
                queryResult.ResultStatus = QueryResultStatus.CriticalError;
            }
            finally
            {
                //log
                _logger.LogDebug("{queryResult} has completed. {widget} returned.", nameof(NewDeckOfCardsQueryResult), queryResult.Deck);
            }
            return queryResult;
        }
    }
}
