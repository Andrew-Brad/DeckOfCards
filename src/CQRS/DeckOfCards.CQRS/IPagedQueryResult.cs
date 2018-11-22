using AB.Extensions;
using System;
using System.Collections.Generic;
using System.Text;

namespace DeckOfCards.CQRS
{
    public interface IPagedQueryResult : IQueryResult
    {
        PagingResponse Paging { get; set; }
    }
}
