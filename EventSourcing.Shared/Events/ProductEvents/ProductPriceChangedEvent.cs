﻿namespace EventSourcing.Shared.Events.ProductEvents
{
    public class ProductPriceChangedEvent : IEvent
    {
        public Guid Id { get; set; }
        public decimal ChangedPrice { get; set; }
    }
}
