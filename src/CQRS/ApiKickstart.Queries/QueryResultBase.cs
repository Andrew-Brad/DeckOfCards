using AB.Extensions;
using ApiKickstart.CQRS;

namespace ApiKickstart.QueryResults
{
    public class QueryResultBase : IQueryResult
    {
        public QueryResultStatus ResultStatus { get; set; }
        public virtual PagingResponse Paging { get; set; } = null;

    }
}
