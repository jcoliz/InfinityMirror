using InfinityMirror.Core.Api;
using InfinityMirror.Core.Features;

namespace InfinityMirror.Endpoint.Controllers;

public class InfinityMirrorControllerImplementation(EventGenerator eventGenerator) : IInfinityMirrorController
{
    private IReadOnlyDictionary<string, IEnumerable<string>> emptyHeaders { get; } = new Dictionary<string, IEnumerable<string>>();

    public Task<SwaggerResponse<DeceptionEventResponse>> GetDeceptionEventsAsync(DateTimeOffset? startingAt, DateTimeOffset? endingAt, int? limit, int? offset, string? user_Agent, string? authorization)
    {
        var events = eventGenerator.GenerateEvents();

        return Task.FromResult(
            new SwaggerResponse<DeceptionEventResponse>(
                StatusCodes.Status200OK,
                emptyHeaders, new()
                {
                    Data = events.ToList()
                }));
    }
}