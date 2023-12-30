using System.Text.Json;
using Microsoft.Azure.Functions.Worker;
using Microsoft.Extensions.Logging;
using Shared;

namespace SampleFunctionApp
{
    public class EventHubTriggeredFunction
    {
        private readonly ILogger _logger;

        public EventHubTriggeredFunction(ILoggerFactory loggerFactory)
        {
            _logger = loggerFactory.CreateLogger<EventHubTriggeredFunction>();
        }

        [Function(nameof(SampleFunction))]
        public void SampleFunction([EventHubTrigger("%EventHubName%", Connection = "EventHubConnection")] EventPayload[] input)
        {
            foreach (var eventData in input)
            {
                _logger.LogInformation($"Event payload received. Content: {JsonSerializer.Serialize(eventData)}");
            }
        }
    }
}
