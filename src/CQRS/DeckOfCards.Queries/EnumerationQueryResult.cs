using System;
using System.Collections.Generic;
using System.Linq;
using DeckOfCards.QueryResults;
using Ardalis.SmartEnum;

namespace DeckOfCards.Queries
{
    public class EnumerationQueryResult<TEnum,TValue> : QueryResultBase 
        where TEnum : SmartEnum<TEnum,TValue>
        where TValue : IEquatable<TValue>, IComparable<TValue>
    {
        public List<TEnum> Enumeration { get; set; }

        public static EnumerationQueryResult<TEnum,TValue> Generate() 
        {
            return new EnumerationQueryResult<TEnum,TValue>()
            {
                Enumeration = SmartEnum<TEnum,TValue>.List.ToList(),
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
