using AB.Extensions;
using System;
using System.Collections.Generic;
using System.Text;

namespace ApiKickstart.CQRS
{
    public interface IPagedQueryResult : IQueryResult
    {
        PagingResponse Paging { get; set; }
    }
}
