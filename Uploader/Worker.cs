using System.Runtime.CompilerServices;
using Azure.Monitor.Ingestion;
using InfinityMirror.Core.Features;
using InfinityMirror.Uploader.Options;
using Microsoft.Extensions.Options;

namespace InfinityMirror.Uploader;

public partial class Worker(
    EventGenerator eventGenerator,
    LogsIngestionClient logsClient,
    IOptions<LogIngestionOptions> logsOptions,
    ILogger<Worker> logger
) : BackgroundService
{
    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        while (!stoppingToken.IsCancellationRequested)
        {
            await SendEventsAsync(stoppingToken);
            logWaiting(_timer.CurrentDelay.TotalSeconds);
            await _timer.WaitAsync(stoppingToken);
        }
    }

    private async Task SendEventsAsync(CancellationToken stoppingToken)
    {
        try
        {
            //
            // Generate events
            //

            var events = eventGenerator.GenerateMessages();
            logGeneratedOk(events.Count);

            //
            // Upload logs
            //

            var response = await logsClient.UploadAsync
            (
                logsOptions.Value.DcrImmutableId,
                logsOptions.Value.Stream,
                [events],
                cancellationToken: stoppingToken
            );
            logUploadedOk(response.Status);
        }
        catch (Exception ex)
        {
            logFail(ex);
        }
    }

    private readonly BackOffTimer _timer = new BackOffTimer(TimeSpan.FromSeconds(2), TimeSpan.FromMinutes(30), 1.3);

    [LoggerMessage(Level = LogLevel.Information, Message = "{Location}: OK", EventId = 1000)]
    public partial void logOk([CallerMemberName] string? location = null);

    [LoggerMessage(Level = LogLevel.Information, Message = "{Location}: OK. Generated {Count} events", EventId = 1001)]
    public partial void logGeneratedOk(int count, [CallerMemberName] string? location = null);

    [LoggerMessage(Level = LogLevel.Information, Message = "{Location}: OK. Uploaded status {Status}", EventId = 1002)]
    public partial void logUploadedOk(int status, [CallerMemberName] string? location = null);

    [LoggerMessage(Level = LogLevel.Information, Message = "{Location}: Waiting {Delay:N1} seconds before next cycle", EventId = 1007)]
    public partial void logWaiting(double delay, [CallerMemberName] string? location = null);

    [LoggerMessage(Level = LogLevel.Error, Message = "{Location}: Failed", EventId = 1008)]
    public partial void logFail(Exception ex, [CallerMemberName] string? location = null);
}
