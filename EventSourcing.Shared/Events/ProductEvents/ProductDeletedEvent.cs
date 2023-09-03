namespace EventSourcing.Shared.Events.ProductEvents
{
    public class ProductDeletedEvent : IEvent
    {
        public Guid Id { get; set; }
    }
}
