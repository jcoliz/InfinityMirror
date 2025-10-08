using InfinityMirror.Core.Models;
using InfinityMirror.Core.Providers;
using Microsoft.Extensions.FileProviders;
using Tomlyn;

namespace InfinityMirror.Core.Helpers;

/// <summary>
/// Loads TOML-formatted generator templates from an IFileProvider
/// </summary>
public class TomlTemplateLoader : IGeneratorTemplateSource
{
    public TomlTemplateLoader(IFileProvider fileProvider)
    {
        // Use fileProvider to load message templates

        // https://stackoverflow.com/questions/62107756/embeddedprovider-getdirectorycontents-returns-0-results
        var directory = fileProvider.GetDirectoryContents("/");
        foreach (var file in directory)
        {
            // TODO: Should ensure the file is a TOML file and is an intended template
            // (e.g. skip README.toml or other non-template files)
            using var stream = file.CreateReadStream();
            using var reader = new StreamReader(stream);
            var toml = reader.ReadToEnd();
            var template = Toml.ToModel<DeceptionEvent>(toml) ?? throw new FormatException($"Unable to parse message template {file.Name}");
            template.Properties ??= new MessageProperties();
            template.Properties.Comment = file.Name;
            Templates[file.Name] = template;
        }
    }

    public Dictionary<string, DeceptionEvent> Templates { get; } = new();
}