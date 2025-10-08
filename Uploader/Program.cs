using System.Reflection;
using Azure.Identity;
using InfinityMirror.Core.Features;
using InfinityMirror.Core.Helpers;
using InfinityMirror.Core.Providers;
using InfinityMirror.Uploader;
using InfinityMirror.Uploader.Options;
using Microsoft.Extensions.Azure;
using Microsoft.Extensions.FileProviders;

var builder = Host.CreateApplicationBuilder(args);

// Add optional TOML config source
builder.Configuration.AddTomlFile("config.toml", optional: true, reloadOnChange: true);

// Add log ingestion options from configuration
builder.Services.Configure<LogIngestionOptions>(builder.Configuration.GetSection(LogIngestionOptions.Section));

// Provide files from embedded resources
builder.Services.AddSingleton<IFileProvider>(new EmbeddedFileProvider(Assembly.GetExecutingAssembly()));

// Generator templates are loaded from embedded resources
builder.Services.AddSingleton<IGeneratorTemplateSource, TomlTemplateLoader>();

// Event generator service
builder.Services.AddSingleton<EventGenerator>();

// Add logs ingestion client
builder.Services.AddAzureClients(clientBuilder =>
{
    // Add a log ingestion client, using endpoint from configuration
    LogIngestionOptions logOptions = new();
    builder.Configuration.Bind(LogIngestionOptions.Section, logOptions);
    clientBuilder.AddLogsIngestionClient(logOptions.EndpointUri);

    // Build a token credential based on the configuration
    IdentityOptions identityOptions = new();
    builder.Configuration.Bind(IdentityOptions.Section, identityOptions);
    var token = new ClientSecretCredential(identityOptions.TenantId.ToString(), identityOptions.AppId.ToString(), identityOptions.AppSecret);

    // Add the desired Azure credential to the client
    clientBuilder.UseCredential(token);
});

// Add the worker hosted service
builder.Services.AddHostedService<Worker>();

var host = builder.Build();
await host.RunAsync();
