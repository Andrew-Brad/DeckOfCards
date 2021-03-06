﻿using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using System.Linq;
using System.Threading;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using DeckOfCards.CQRS;
using DeckOfCards.Domain;
using DeckOfCards.QueryResults;
using DeckOfCards.Queries;
using AB.Extensions;
using AutoMapper;
using MediatR;
using Sieve.Services;
using Sieve.Models;
using Polly;
using Polly.Registry;
using Raven.Client.Documents;
using Raven.Client.Documents.Session;
using Microsoft.Extensions.Caching.Memory;

namespace DeckOfCards.QueryHandlers
{
    public class CardTemplateQueryHandler : IRequestHandler<CardTemplateQuery, CardTemplateQueryResult>
    {
        private readonly ILogger<CardTemplateQueryHandler> _logger;
        private readonly IDocumentStore _docStore;
        private readonly IMapper _mapper;
        private readonly ISieveProcessor _sortFilterPagingProcessor;
        private readonly IOptions<SieveOptions> _sortFilterPagingOptions;
        private readonly IReadOnlyPolicyRegistry<string> _policyRegistry;

        public CardTemplateQueryHandler(ILogger<CardTemplateQueryHandler> logger, IDocumentStore context, IMapper mapper,
            ISieveProcessor sieveProc, IOptions<SieveOptions> sortFilterPagingOptions, IReadOnlyPolicyRegistry<string> registry)
        {
            _logger = logger;
            _docStore = context;
            _mapper = mapper;
            _sortFilterPagingProcessor = sieveProc;
            _sortFilterPagingOptions = sortFilterPagingOptions;
            _policyRegistry = registry;
        }

        public async Task<CardTemplateQueryResult> Handle(CardTemplateQuery query, CancellationToken cancellationToken)
        {
            var queryResult = new CardTemplateQueryResult();
            try
            {
                var policy = _policyRegistry.Get<IAsyncPolicy<int>>("DbQuery");
                CardTemplate card = null;
                var policyResult = await policy
                    //.ExecuteAndCaptureAsync(StubMethod)
                    .ExecuteAndCaptureAsync(async () =>
                    {
                        using (var session = _docStore.OpenAsyncSession(new SessionOptions() { NoTracking = true }))
                        {
                            var allDbWidgetsQuery = session
                                .Query<CardTemplate>()
                                .Where(x => x.Rank == query.Rank.Value && x.Suit == query.Suit.Value);

                            //allDbWidgetsQuery = _sortFilterPagingProcessor.Apply(query.SortFilterPaging, allDbWidgetsQuery, null, false, false, true);
                            card = await allDbWidgetsQuery.SingleAsync();
                            return 1;
                        }
                    });
                //if (policyResult.Outcome == OutcomeType.Failure) return ServiceUnavailableCommandResult();

                //queryResult.Paging = new PagingResponse((uint)query.SortFilterPaging.Page.Value,(uint) queryResult.Widgets.Count, (uint)totalWidgetCount);
                //queryResult.Paging.TotalPages = (uint)GetTotalPages(query.SortFilterPaging, totalWidgetCount);
                queryResult.Card = card;
                queryResult.ResultStatus = QueryResultStatus.SuccessfullyProcessed;
                _logger.LogTrace("{queryResult} has completed. {@cardTemplate} returned.", queryResult, queryResult.Card);
            }
            catch (Exception e)
            {
                _logger.LogError(e, "Exception caught and logged.");
                queryResult.ResultStatus = QueryResultStatus.CriticalError;
            }
            finally
            {
                //log

            }
            return queryResult;
        }

        public int GetTotalPages(SieveModel pagingModel, int totalWidgetCount)
        {
            int safePageSize = _sortFilterPagingOptions.Value.DefaultPageSize;
            if (pagingModel.PageSize.HasValue) safePageSize = pagingModel.PageSize.Value;
            return (int)Math.Ceiling(((decimal)totalWidgetCount / safePageSize));
        }
    }
}
