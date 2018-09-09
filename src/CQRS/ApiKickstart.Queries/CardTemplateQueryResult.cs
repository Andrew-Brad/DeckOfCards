using System.Collections.Generic;
using ApiKickstart.CQRS;
using ApiKickstart.Domain;
using AB.Extensions;

namespace ApiKickstart.QueryResults
{
    public class CardTemplateQueryResult : QueryResultBase, IQueryResult//IPagedQueryResult
    {
        public CardTemplate Card { get; set; }
        //public override PagingResponse Paging { get; set; }
    }
}
