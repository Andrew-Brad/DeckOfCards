namespace ApiKickstart.DomainEvents
{
    public class WidgetCreatedV1Event : EventBase
    {
        public int WidgetId { get; set; }
        public override string EventName { get; set; } = nameof(WidgetDeprecatedV1Event);
        public override int EventVersion { get; set; } = 1;
    }
}
