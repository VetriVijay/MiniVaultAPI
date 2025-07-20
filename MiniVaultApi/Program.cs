using MiniVaultApi.Models;
using MiniVaultApi.Services;
using Serilog;
using Serilog.Formatting.Compact;
using System.Text;
using Microsoft.OpenApi.Models;


var builder = WebApplication.CreateBuilder(args);

// Configure Serilog
Serilog.Debugging.SelfLog.Enable(Console.Out);

Log.Logger = new LoggerConfiguration()
    .MinimumLevel.Information()
    .WriteTo.File(path: "logs/log.jsonl",
        formatter: new RenderedCompactJsonFormatter(),
        rollingInterval: RollingInterval.Day)
    .CreateLogger();

builder.Host.UseSerilog();;

// Register in DI Container
builder.Services.AddHttpClient<IGenerateService, GenerateService>();

builder.Services.Configure<LlmSettings>(builder.Configuration.GetSection("LlmSettings"));

builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen(c =>
{
    c.SwaggerDoc("v1", new Microsoft.OpenApi.Models.OpenApiInfo
    {
        Title = "MiniVault API",
        Version = "v1"
    });
});

var app = builder.Build();

app.UseSwagger();
app.UseSwaggerUI(c =>
{
    c.SwaggerEndpoint("/swagger/v1/swagger.json", "MiniVault API v1");
});

app.MapPost("/generate", async (GenerateRequest req, IGenerateService service) =>
{
    var result = await service.GenerateAsync(req);
    return Results.Ok(result);
})
.WithName("Generate");

app.MapPost("/generate/stream", async (GenerateRequest req, IGenerateService service) =>
{
    var stream = service.StreamGenerateAsync(req);

    return Results.Stream(async (streamWriter) =>
    {
        await foreach (var token in stream)
        {
            var bytes = Encoding.UTF8.GetBytes(token);
            await streamWriter.WriteAsync(bytes);
            await streamWriter.FlushAsync();
        }
    }, "text/plain");
})
.WithName("GenerateStream");

app.MapGet("/", () => "MiniVault API is running!");

app.Run();
