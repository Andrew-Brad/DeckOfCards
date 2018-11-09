using System.Collections.Generic;
using DeckOfCards.CQRS;
using DeckOfCards.Domain;
using AB.Extensions;

namespace DeckOfCards.QueryResults
{
    public class CardTemplateQueryResult : QueryResultBase, IQueryResult//IPagedQueryResult
    {
        public CardTemplate Card { get; set; }
        //public override PagingResponse Paging { get; set; }
    }
}
