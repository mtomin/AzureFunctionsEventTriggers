using System.Net.Mime;
using System.Text;
using System.Text.Json;
using EventEmitter.EventEmitter.Configuration;
using Microsoft.Extensions.Options;

namespace EventEmitter.EventEmitter
{
    internal class EventSimulator : IEventEmitter
    {
        private readonly HttpClient client;
        private readonly string adminEndpoint;

        public EventSimulator(IOptions<EventSimulatorSettings> settings)
        {
            client = new HttpClient();
            adminEndpoint = $"{settings.Value.BaseUrl}/admin/functions/{settings.Value.FunctionName}";
        }

        public async Task EmitAsync<T>(T payload) => await client.PostAsync(adminEndpoint, SerializeContent(payload));

        public async Task EmitAsync<T>(IEnumerable<T> payloads)
        {
            if (payloads?.Count() > 0)
            {
                var tasks = new List<Task>();
                foreach (var payload in payloads)
                {
                    tasks.Add(client.PostAsync(adminEndpoint, SerializeContent(payload)));
                }
                await Task.WhenAll(tasks);
            }
        }

        private static StringContent SerializeContent<E>(E content) => new(JsonSerializer.Serialize(new { input = JsonSerializer.Serialize(content) }), Encoding.UTF8, MediaTypeNames.Application.Json);
    }
}
