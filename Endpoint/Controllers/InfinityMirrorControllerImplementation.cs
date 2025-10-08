using InfinityMirror.Core.Api;

namespace InfinityMirror.Endpoint.Controllers;

public class InfinityMirrorControllerImplementation : IInfinityMirrorController
{
    private IReadOnlyDictionary<string, IEnumerable<string>> emptyHeaders { get; } = new Dictionary<string, IEnumerable<string>>();

    public Task<SwaggerResponse<DeceptionEventResponse>> GetDeceptionEventsAsync(DateTimeOffset? startingAt, DateTimeOffset? endingAt, int? limit, int? offset, string? user_Agent, string? authorization)
    {
        return Task.FromResult(new SwaggerResponse<DeceptionEventResponse>(StatusCodes.Status200OK, emptyHeaders, new()));
    }
}