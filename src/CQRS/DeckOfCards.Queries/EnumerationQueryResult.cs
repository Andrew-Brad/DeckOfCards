using ApiKickstart.QueryResults;
using System.Collections.Generic;
using Ardalis.SmartEnum;

namespace ApiKickstart.Queries
{
    public class EnumerationQueryResult<TEnum,TValue> : QueryResultBase where TEnum : SmartEnum<TEnum,TValue>
    {
        public List<TEnum> Enumeration { get; set; }

        public static EnumerationQueryResult<TEnum,TValue> Generate() 
        {
            return new EnumerationQueryResult<TEnum,TValue>()
            {
                Enumeration = SmartEnum<TEnum,TValue>.List,
                ResultStatus = CQRS.QueryResultStatus.SuccessfullyProcessed
            };
        }
    }

    //public class EnumerationQueryResult2 : QueryResultBase
    //{
    //    public List<WidgetClassification> Enumeration { get; set; }

    //    public static EnumerationQueryResult2 Generate()
    //    {
    //        return new EnumerationQueryResult2()
    //        {
    //            Enumeration = WidgetClassification.List,
    //            ResultStatus = CQRS.QueryResultStatus.SuccessfullyProcessed
    //        };
    //    }
    //}
}
