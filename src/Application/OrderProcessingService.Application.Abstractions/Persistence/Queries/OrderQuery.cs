namespace OrderProcessingService.Application.Abstractions.Persistence.Queries;

public record OrderQuery(
    long[]? OrderIds,
    int PageSize,
    long? Cursor);