using System;
using System.Collections.Generic;
using System.Text;

namespace ApiKickstart.CQRS
{
    public interface IQueryResult
    {
        QueryResultStatus ResultStatus { get; set; }
    }

    public enum QueryResultStatus
    {
        NotYetProcessed = 0,
        SuccessfullyProcessed = 1,
        NoResultData = 2,
        ServiceUnavailable = 3,
        CriticalError = 4
    }
}
