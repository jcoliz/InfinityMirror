namespace InfinityMirror.Core.Providers;

/// <summary>
/// Source of templates for the event generator
/// </summary>
public interface IGeneratorTemplateSource
{
    /// <summary>
    /// Templates to use for generating messages
    /// </summary>
    Dictionary<string, Api.DeceptionEvent> ApiTemplates { get; }
}
