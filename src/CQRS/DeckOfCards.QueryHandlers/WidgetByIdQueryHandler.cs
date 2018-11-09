using DeckOfCards.QueryResults;
using DeckOfCards.Queries;
using MediatR;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using System.Linq;
using DeckOfCards.CQRS;
using Microsoft.Extensions.Logging;
using System.Threading;
using AutoMapper;
using Raven.Client.Documents;
using DeckOfCards.Domain;

namespace DeckOfCards.QueryHandlers
{
    public class WidgetByIdQueryHandler : IRequestHandler<WidgetByIdQuery, WidgetByIdQueryResult>
    {
        private readonly ILogger<WidgetByIdQueryHandler> _logger;
        private readonly IDocumentStore _context;
        private readonly IMapper _mapper;

        public WidgetByIdQueryHandler(ILogger<WidgetByIdQueryHandler> logger, IDocumentStore context, IMapper mapper)
        {
            _logger = logger;
            _context = context;
            _mapper = mapper;
        }

        public async Task<WidgetByIdQueryResult> Handle(WidgetByIdQuery query, CancellationToken cancellationToken)
        {
            var queryResult = new WidgetByIdQueryResult();
            try
            {
                using (var session = _context.OpenAsyncSession())
                {
                    var widget = await session.LoadAsync<CardTemplate>(query.WidgetId);
                    queryResult.Widget = _mapper.Map<Domain.CardTemplate>(widget);
                }

                queryResult.ResultStatus = queryResult.Widget != null ? QueryResultStatus.SuccessfullyProcessed : QueryResultStatus.NoResultData;//potential to wrap this into its own generic extension method looking at Result<T>
                _logger.LogTrace("{queryResult} has completed. {widget} returned.", nameof(WidgetByIdQueryResult), queryResult.Widget.CardName);
            }
            catch (Exception e)
            {
                _logger.LogError(e, "Error occurred on query.");
                queryResult.ResultStatus = QueryResultStatus.CriticalError;
            }
            finally
            {
                //log
                _logger.LogDebug("{queryResult} has completed. {widget} returned.", nameof(WidgetByIdQueryResult), queryResult.Widget);
            }
            return queryResult;
        }
    }
}
