using InfinityMirror.Core.Models;
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
    public ICollection<DeceptionEvent> GenerateMessages()
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
                messages.AddRange(Enumerable.Range(1, genProps.MessagesPerInterval).Select(x => GenerateMessage(template, x)));
            }
        }
        HasRunThisSession = true;
        return messages;
    }

    private DeceptionEvent GenerateMessage(DeceptionEvent template, int SequenceNumber)
    {
        return template with
        {
            TimeOnClient = DateTimeOffset.UtcNow,
            Id = Guid.NewGuid().ToString(),
            Properties = template.Properties with
            {
                SessionId = SessionId,
                SequenceNumber = SequenceNumber
            }
        };
    }
}