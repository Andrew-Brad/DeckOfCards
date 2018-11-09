using DeckOfCards.CQRS;
using MediatR;
using System;

namespace DeckOfCards.DomainEvents
{
    /// <summary>
    /// todo: notes - one required constructor - a V2 Event should be able to construct from a V1 event etc...
    /// </summary>
    public class WidgetDeprecatedV1Event : EventBase
    {
        public int WidgetId { get; set; }
        public override string EventName { get; set; } = nameof(WidgetDeprecatedV1Event);
        public override int EventVersion { get; set; } = 1;
    }
}
