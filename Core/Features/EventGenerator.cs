using System.Text.Json;
using InfinityMirror.Core.Providers;

namespace InfinityMirror.Core.Features;

/// <summary>
/// Generates synthetic events based on templates
/// </summary>
/// <param name="generatorTemplateSource">Where to look for templates</param>
public class EventGenerator(IGeneratorTemplateSource generatorTemplateSource)
{
    private Guid SessionId { get; } = Guid.NewGuid();

    private bool HasRunThisSession { get; set; } = false;

    /// <summary>
    /// Generate messages based on the templates
    /// </summary>
    /// <returns>Collection of events</returns>
    public ICollection<Api.DeceptionEvent> GenerateApiMessages()
    {
        var messages = new List<Api.DeceptionEvent>();

        foreach (var template in generatorTemplateSource.ApiTemplates.Values)
        {
            var genProps = template.Properties.Generation;
            if (
                (genProps?.Interval == Api.GenerationInterval.Cycle)
                ||
                (genProps?.Interval == Api.GenerationInterval.Session && !HasRunThisSession)
            )
            {
                messages.AddRange(Enumerable.Range(1, genProps.MessagesPerInterval ?? 1).Select(x => GenerateApiMessage(template, x)));
            }
        }
        HasRunThisSession = true;
        return messages;
    }
    
    private Api.DeceptionEvent GenerateApiMessage(Api.DeceptionEvent template, int SequenceNumber)
    {
        var result = JsonSerializer.Deserialize<Api.DeceptionEvent>(JsonSerializer.Serialize(template)) ?? throw new InvalidOperationException("Failed to clone template");

        result.TimeOnClient = DateTimeOffset.UtcNow;
        result.Id = Guid.NewGuid().ToString();
        result.Properties ??= new Api.MessageProperties();
        result.Properties.SessionId = SessionId;
        result.Properties.SequenceNumber = SequenceNumber;

        return result;
    }
}