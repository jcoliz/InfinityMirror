using System.Text.Json;
using InfinityMirror.Core.Api;
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
    public ICollection<DeceptionEvent> GenerateEvents()
    {
        var messages = new List<DeceptionEvent>();

        foreach (var template in generatorTemplateSource.Templates.Values)
        {
            var genProps = template.Properties.Generation;
            if (
                (genProps?.Interval == GenerationInterval.Cycle)
                ||
                (genProps?.Interval == GenerationInterval.Session && !HasRunThisSession)
            )
            {
                messages.AddRange(Enumerable.Range(1, genProps.MessagesPerInterval ?? 1).Select(x => GenerateSingleEvent(template, x)));
            }
        }
        HasRunThisSession = true;
        return messages;
    }

    private DeceptionEvent GenerateSingleEvent(DeceptionEvent template, int SequenceNumber)
    {
        var result = JsonSerializer.Deserialize<DeceptionEvent>(JsonSerializer.Serialize(template)) ?? throw new InvalidOperationException("Failed to clone template");

        result.TimeOnClient = DateTimeOffset.UtcNow;
        result.Id = Guid.NewGuid().ToString();
        result.Properties ??= new MessageProperties();
        result.Properties.SessionId = SessionId;
        result.Properties.SequenceNumber = SequenceNumber;

        return result;
    }
}