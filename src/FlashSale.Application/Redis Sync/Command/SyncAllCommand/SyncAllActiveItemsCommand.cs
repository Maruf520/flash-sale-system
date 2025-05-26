using FluentValidation;
using MediatR;

public record SyncAllActiveItemsCommand(string? TriggeredBy, bool ForceSync = false) : IRequest<SyncAllActiveItemsResult>;

public record SyncAllActiveItemsResult(
    bool Success,
    string Message,
    long DurationMs,
    DateTime Timestamp,
    int ItemsProcessed,
    int SuccessfulSyncs,
    int FailedSyncs,
    List<string> Errors
);

public class SyncAllActiveItemsCommandValidator : AbstractValidator<SyncAllActiveItemsCommand>
{
    public SyncAllActiveItemsCommandValidator()
    {
        RuleFor(x => x.TriggeredBy)
            .NotEmpty()
            .WithMessage("TriggeredBy is required for audit purposes.");
    }
}