namespace OrderProcessingService.Application.Abstractions.Persistence.Queries;

public record OrderQuery(
    long[]? OrderIds,
    long Cursor,
    int PageSize);
