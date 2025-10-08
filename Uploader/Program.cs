using System.Reflection;
using InfinityMirror.Core.Features;
using InfinityMirror.Core.Helpers;
using InfinityMirror.Core.Providers;
using InfinityMirror.Uploader;
using Microsoft.Extensions.FileProviders;

var builder = Host.CreateApplicationBuilder(args);
builder.Services.AddHostedService<Worker>();

// Provide files from embedded resources
builder.Services.AddSingleton<IFileProvider>(new EmbeddedFileProvider(Assembly.GetExecutingAssembly()));

// Generator templates are loaded from embedded resources
builder.Services.AddSingleton<IGeneratorTemplateSource, TomlTemplateLoader>();

// Event generator service
builder.Services.AddSingleton<EventGenerator>();

var host = builder.Build();
host.Run();
