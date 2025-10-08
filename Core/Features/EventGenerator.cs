using System.Text.Json;
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