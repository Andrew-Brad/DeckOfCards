namespace DeckOfCards.DomainEvents
{
    public class DeckCreatedV1Event : EventBase
    {
        public int DeckId { get; set; }
        public override string EventName { get; set; } = nameof(DeckCreatedV1Event);
        public override int EventVersion { get; set; } = 1;
    }
}
