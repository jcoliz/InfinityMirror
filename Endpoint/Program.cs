using System.Reflection;
using InfinityMirror.Core.Api;
using InfinityMirror.Core.Features;
using InfinityMirror.Core.Helpers;
using InfinityMirror.Core.Providers;
using InfinityMirror.Core.Implementations;
using InfinityMirror.Endpoint.Helpers;
using Microsoft.Extensions.FileProviders;
using NSwag;
using NSwag.Generation.Processors.Security;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.

// Add controllers with custom JSON options
builder.Services.AddControllers().AddJsonOptions(options => {
    options.JsonSerializerOptions.PropertyNameCaseInsensitive = false;
    options.JsonSerializerOptions.WriteIndented = true;
    options.JsonSerializerOptions.DefaultIgnoreCondition = System.Text.Json.Serialization.JsonIgnoreCondition.WhenWritingDefault;
    options.JsonSerializerOptions.Converters.Add(new DateTimeOffsetJsonConverter());
}
);

// Add OpenAPI/Swagger
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddOpenApiDocument(options =>
{
    options.Title = "InfinityMirror.Endpoint";
    options.Description = "Infinity Mirror Service API";
    options.AddSecurity("ApiKeyAuth", new OpenApiSecurityScheme
    {
        In = OpenApiSecurityApiKeyLocation.Header,
        Description = "Enter your token",
        Name = "X-API-Key",
        Type = OpenApiSecuritySchemeType.ApiKey
    });
    options.OperationProcessors.Add(new OperationSecurityScopeProcessor("ApiKeyAuth"));
});

// Provide files from embedded resources
builder.Services.AddSingleton<IFileProvider>(new EmbeddedFileProvider(Assembly.GetExecutingAssembly()));

// Generator templates are loaded from embedded resources
builder.Services.AddSingleton<IGeneratorTemplateSource, TomlTemplateLoader>();

// Event generator service
builder.Services.AddSingleton<EventGenerator>();

// Add controller implementations
builder.Services.AddSingleton<IInfinityMirrorController, InfinityMirrorControllerImplementation>();

var app = builder.Build();

// Add swagger middleware
app.UseOpenApi();
app.UseSwaggerUi();

app.UseHttpsRedirection();

app.MapControllers();

await app.RunAsync();
