using Itmo.Dev.Platform.Events;
using OrderProcessingService.Application.Services.Extensions;
using OrderProcessingService.Infrastructure.Persistence.Extensions;
using OrderProcessingService.Presentation.Grpc;
using OrderProcessingService.Presentation.Kafka;
using OrderProcessingService.Presentation.Kafka.Extensions;

WebApplicationBuilder builder = WebApplication.CreateBuilder(args);

builder.Services.AddPlatformEvents(e => e.AddPresentationKafkaEventHandlers());
builder.Services.AddApplication();
builder.Services.AddPersistencePostgres();
builder.Services.AddPersistenceMigrations();
builder.Services.AddPresentationKafka(builder.Configuration);
builder.Services.AddPresentationGrpc();

WebApplication app = builder.Build();

app.UseRouting();
app.UsePresentationGrpc();

await app.RunAsync();