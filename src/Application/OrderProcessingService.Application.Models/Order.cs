namespace OrderProcessingService.Application.Models;

public record Order(long Id, OrderState State, DateTimeOffset CreatedAt, DateTimeOffset UpdatedAt);