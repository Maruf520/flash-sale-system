public record SyncSpecificItemCommand(
    Guid FlashSaleItemId,
    string? TriggeredBy,
    bool ForceSync = false
) : IRequest<SyncSpecificItemResult>;

public record SyncSpecificItemResult(
    bool Success,
    string Message,
    long DurationMs,
    DateTime Timestamp,
    Guid FlashSaleItemId,
    string? ProductName,
    int? DatabaseStock,
    int? RedisStock,
    string? Error
);

public class SyncSpecificItemCommandValidator : AbstractValidator<SyncSpecificItemCommand>
{
    public SyncSpecificItemCommandValidator()
    {
        RuleFor(x => x.FlashSaleItemId)
            .NotEmpty()
            .WithMessage("FlashSaleItemId is required.");

        RuleFor(x => x.TriggeredBy)
            .NotEmpty()
            .WithMessage("TriggeredBy is required for audit purposes.");
    }
}