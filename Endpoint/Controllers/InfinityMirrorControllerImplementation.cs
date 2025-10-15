using InfinityMirror.Core.Api;
using InfinityMirror.Core.Features;

namespace InfinityMirror.Endpoint.Controllers;

public class InfinityMirrorControllerImplementation(EventGenerator eventGenerator) : IInfinityMirrorController
{
    private IReadOnlyDictionary<string, IEnumerable<string>> emptyHeaders { get; } = new Dictionary<string, IEnumerable<string>>();

    public Task<SwaggerResponse<DeceptionEventResponse>> GetDeceptionEventsAsync(DateTimeOffset? startingAt, DateTimeOffset? endingAt, int? limit, int? offset, string? user_Agent, string? apiKey)
    {
        var events = eventGenerator.GenerateEvents();

        foreach(var ev in events)
        {
            ev.Properties.UserAgent = user_Agent;
            ev.Properties.ApiKey = apiKey;
        }

        return Task.FromResult(
            new SwaggerResponse<DeceptionEventResponse>(
                StatusCodes.Status200OK,
                emptyHeaders, new()
                {
                    Data = events.ToList()
                }));
    }
}