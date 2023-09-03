namespace EventSourcing.Shared.Events.ProductEvents
{
    public class ProductNameChangedEvent : IEvent
    {
        public Guid Id { get; set; }
        public string ChangedName { get; set; }
    }
}
