using EventStore.ClientAPI;

namespace EventSourcing.Api.Extentions
{
    public static class EventStoreExtentions
    {
        public static void AddEventStore(this IServiceCollection services, IConfiguration configuration)
        {
            var connection = EventStoreConnection.Create(connectionString: configuration.GetConnectionString("EventStoreConnection"));

            connection.ConnectAsync().Wait();

            services.AddSingleton(connection);

            using var logFactory = LoggerFactory.Create(builder =>
            {
                builder.SetMinimumLevel(LogLevel.Information);
                builder.AddConsole();
            });

            var logger = logFactory.CreateLogger("Program");

            connection.Connected += (sender, args) =>
            {
                logger.LogInformation("EventStore connection established");
            };

            connection.ErrorOccurred += (sender, args) =>
            {
                logger.LogError(args.Exception.Message);
            };
        }
    }
}
