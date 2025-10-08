using InfinityMirror.Core.Api;
using InfinityMirror.Endpoint.Controllers;
using InfinityMirror.Endpoint.Helpers;

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
});

// Add controller implementations
builder.Services.AddSingleton<IInfinityMirrorController, InfinityMirrorControllerImplementation>();

var app = builder.Build();

// Add swagger middleware
app.UseOpenApi();
app.UseSwaggerUi();

app.UseHttpsRedirection();

app.MapControllers();

await app.RunAsync();
