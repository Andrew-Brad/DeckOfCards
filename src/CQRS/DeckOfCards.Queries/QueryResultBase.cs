using AB.Extensions;
using DeckOfCards.CQRS;

namespace DeckOfCards.QueryResults
{
    public class QueryResultBase : IQueryResult
    {
        public QueryResultStatus ResultStatus { get; set; }
        public virtual PagingResponse Paging { get; set; } = null;

    }
}
