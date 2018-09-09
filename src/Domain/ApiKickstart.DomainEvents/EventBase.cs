using ApiKickstart.CQRS;
using MediatR;
using System;

namespace ApiKickstart.DomainEvents
{
    public abstract class EventBase : IEvent, INotification
    {
        public virtual Guid BroadcastingApplicationId { get; set; }
        public virtual DateTime BroadcastDateTime { get; set; } = DateTime.UtcNow;
        public abstract string EventName { get; set; }
        public abstract int EventVersion { get; set; }
    }
}