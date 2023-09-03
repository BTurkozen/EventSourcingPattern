using EventSourcing.Api.EventStores;
using EventSourcing.Api.Models;
using EventSourcing.Shared.Events.ProductEvents;
using EventStore.ClientAPI;
using System.Text;
using System.Text.Json;

namespace EventSourcing.Api.BackgroundServices
{
    public class ProductReadModelEventStore : BackgroundService
    {
        private readonly IEventStoreConnection _eventStoreConnection;
        private readonly ILogger<ProductReadModelEventStore> _logger;
        private readonly IServiceProvider _serviceProvider;

        public ProductReadModelEventStore(IEventStoreConnection eventStoreConnection, ILogger<ProductReadModelEventStore> logger, IServiceProvider serviceProvider)
        {
            _eventStoreConnection = eventStoreConnection;
            _logger = logger;
            _serviceProvider = serviceProvider;
        }

        public override Task StartAsync(CancellationToken cancellationToken)
        {
            return base.StartAsync(cancellationToken);
        }

        public override Task StopAsync(CancellationToken cancellationToken)
        {
            return base.StopAsync(cancellationToken);
        }

        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            // autoAck: true => EventAppeared exception fırlatmadı ise event gönderildi sayar.
            // autoAck: false => Bunu biz method içerisinde elle yapıyoruz. arg1.Acknowledge(arg2.Event.EventId); bu işlemi false yazma işleminde gerçekleştiririz.
            await _eventStoreConnection.ConnectToPersistentSubscriptionAsync(ProductStream.StreamName, ProductStream.GroupName, EventAppeared, autoAck: false);

            throw new NotImplementedException();
        }

        private async Task EventAppeared(EventStorePersistentSubscriptionBase arg1, ResolvedEvent arg2)
        {
            _logger.LogInformation("The Message processing...");

            // Gelen event'ın tipini alıyoruz.
            // MetaData tipleri ayrı class library'de olduğu için böyle bir kod ekliyoruz.
            var type = Type.GetType($"{Encoding.UTF8.GetString(arg2.Event.Metadata)}, EventSourcing.Shared");

            var eventData = Encoding.UTF8.GetString(arg2.Event.Data);

            var @event = JsonSerializer.Deserialize(eventData, type);

            using var scope = _serviceProvider.CreateScope();

            var dataContext = scope.ServiceProvider.GetRequiredService<DataContext>();

            Product product = null;

            switch (@event)
            {
                case ProductCreatedEvent productCreatedEvent:

                    product = new Product()
                    {
                        Id = productCreatedEvent.Id,
                        Name = productCreatedEvent.Name,
                        Price = productCreatedEvent.Price,
                        Stock = productCreatedEvent.Stock,
                        UserId = productCreatedEvent.UserId,
                    };

                    break;

                case ProductNameChangedEvent productNameChangedEvent:

                    product = await dataContext.Products.FindAsync(productNameChangedEvent.Id);

                    if (product is not null)
                    {
                        product.Name = productNameChangedEvent.ChangedName;
                    }

                    break;

                case ProductPriceChangedEvent productPriceChangedEvent:

                    product = await dataContext.Products.FindAsync(productPriceChangedEvent.Id);

                    if (product is not null)
                    {
                        product.Price = productPriceChangedEvent.ChangedPrice;
                    }

                    break;

                case ProductDeletedEvent productDeletedEvent:

                    product = await dataContext.Products.FindAsync(productDeletedEvent.Id);

                    if (product is not null)
                    {
                        dataContext.Products.Remove(product);
                    }

                    break;
            }

            await dataContext.SaveChangesAsync();

            arg1.Acknowledge(arg2.Event.EventId);
        }
    }
}
