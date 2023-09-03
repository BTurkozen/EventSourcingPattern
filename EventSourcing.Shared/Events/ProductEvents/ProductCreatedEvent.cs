namespace EventSourcing.Shared.Events.ProductEvents
{
    public class ProductCreatedEvent : IEvent
    {
        public Guid Id { get; set; }
        public string Name { get; set; }
        public int UserId { get; set; }
        public decimal Price { get; set; }
        public int Stock { get; set; }
    }
}
