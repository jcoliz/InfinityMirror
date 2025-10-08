using InfinityMirror.Core.Features;

namespace InfinityMirror.Uploader;

public partial class Worker(EventGenerator eventGenerator, ILogger<Worker> logger) : BackgroundService
{
    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        while (!stoppingToken.IsCancellationRequested)
        {
            await SendEventsAsync(stoppingToken);

            await Task.Delay(1000, stoppingToken);
        }
    }

    private Task SendEventsAsync(CancellationToken stoppingToken)
    {
        var events = eventGenerator.GenerateMessages();
        foreach (var ev in events)
        {
            logger.LogInformation("Generated event: {EventId} at {EventTime}", ev.Id, ev.TimeOnClient);
        }
        return Task.CompletedTask;
    }
}
