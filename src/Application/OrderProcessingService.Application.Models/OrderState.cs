namespace OrderProcessingService.Application.Models;

public enum OrderState
{
    Created,
    PendingApproval,
    Approved,
    Packing,
    Packed,
    InDelivery,
    Delivered,
    Cancelled,
}