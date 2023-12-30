using EventEmitter.EventEmitter;
using EventEmitter.EventEmitter.Configuration;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Shared;

using var host = ConfigureHost();
int eventNumber = 0;
string? input;
const int batchSize = 5;

do
{
    Console.WriteLine("Emit event? S(ingle)/M(ultiple)/E(nd)");
    input = Console.ReadLine();
    if (string.Equals(input, "s", StringComparison.OrdinalIgnoreCase))
    {
        var eventPayload = new EventPayload($"Event{eventNumber}");
        await host.Services.GetRequiredService<IEventEmitter>().EmitAsync(eventPayload);
        eventNumber++;
    }
    else if (string.Equals(input, "m", StringComparison.OrdinalIgnoreCase))
    {
        var eventBatch = Enumerable.Range(eventNumber, batchSize).Select(index => new EventPayload($"Event{index}"));
        await host.Services.GetRequiredService<IEventEmitter>().EmitAsync(eventBatch);
        eventNumber += batchSize;
    }
} while (!string.Equals(input, "e", StringComparison.OrdinalIgnoreCase));

IHost ConfigureHost()
{
    HostApplicationBuilder builder = Host.CreateApplicationBuilder(args);

    if (builder.Configuration.GetValue<bool>("UseEventSimulator"))
    {
        builder.Services.Configure<EventSimulatorSettings>(builder.Configuration.GetSection(nameof(EventSimulatorSettings)));
        builder.Services.AddScoped<IEventEmitter, EventSimulator>();
    }
    else
    {
        builder.Services.Configure<EventHubSettings>(builder.Configuration.GetSection(nameof(EventHubSettings)));
        builder.Services.AddScoped<IEventEmitter, EventHubEventEmitter>();
    }

    return builder.Build();
}
