using EventSourcing.Shared.Events;
using EventStore.ClientAPI;
using System.Text;
using System.Text.Json;

namespace EventSourcing.Api.EventStores
{
    public abstract class AbstractStream
    {
        protected readonly LinkedList<IEvent> Events = new LinkedList<IEvent>();
        private string _streamName { get; }
        private readonly IEventStoreConnection _eventStoreConnection;

        protected AbstractStream(string streamName, IEventStoreConnection eventStoreConnection)
        {
            _streamName = streamName;
            _eventStoreConnection = eventStoreConnection;
        }

        public async Task SaveAsync()
        {
            var newEvent = Events.ToList()
                                 .Select(e =>
                                 new EventData(
                                     Guid.NewGuid(),
                                     e.GetType().Name,
                                     true,
                                     Encoding.UTF8.GetBytes(JsonSerializer.Serialize(e, inputType: e.GetType())),
                                     Encoding.UTF8.GetBytes(e.GetType().FullName)
                                     )).ToList();

            // stream'e ekleme işlemi yapıyoruz.
            await _eventStoreConnection.AppendToStreamAsync(_streamName, ExpectedVersion.Any, newEvent);

            // Kayıt ettikten sonra event'leri temizlememiz gerekiyor.
            Events.Clear();
        }
    }
}
