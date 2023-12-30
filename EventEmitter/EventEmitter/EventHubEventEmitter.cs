using Azure.Messaging.EventHubs;
using Azure.Messaging.EventHubs.Producer;
using EventEmitter.EventEmitter.Configuration;
using Microsoft.Extensions.Options;
using System.Text.Json;

namespace EventEmitter.EventEmitter
{
    internal class EventHubEventEmitter : IEventEmitter
    {
        private readonly EventHubProducerClient client;

        public EventHubEventEmitter(IOptions<EventHubSettings> eventHubSettings)
        {
            client = new EventHubProducerClient(eventHubSettings.Value.Connection, eventHubSettings.Value.Name);
        }

        public async Task EmitAsync<T>(T payload)
        {
            var eventData = new EventData(JsonSerializer.Serialize(payload));
            await client.SendAsync(new EventData[] { eventData });
        }

        public async Task EmitAsync<T>(IEnumerable<T> payloads)
        {
            if (!payloads?.Any() ?? true)
            {
                return;
            }

            using EventDataBatch eventBatch = await client.CreateBatchAsync();

            foreach (var payload in payloads!)
            {
                {
                    eventBatch.TryAdd(new EventData(JsonSerializer.Serialize(payload)));
                }
            }

            await client.SendAsync(eventBatch);
        }
    }
}