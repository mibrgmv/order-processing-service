using OrderProcessingService.Application.Services.Extensions;
using OrderProcessingService.Infrastructure.Events;
using OrderProcessingService.Infrastructure.Persistence.Extensions;
using OrderProcessingService.Presentation.Grpc;
using OrderProcessingService.Presentation.Kafka;

WebApplicationBuilder builder = WebApplication.CreateBuilder(args);

builder.Services.AddPlatformEvents(e => e.AddPresentationKafkaEventHandlers());
builder.Services.AddApplication();
builder.Services.AddPersistence();
builder.Services.AddPresentationGrpc();

WebApplication app = builder.Build();

app.UseRouting();
app.UsePresentationGrpc();

await app.RunAsync();