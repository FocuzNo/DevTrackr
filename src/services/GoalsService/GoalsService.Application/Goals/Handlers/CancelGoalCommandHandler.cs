using DevTrackr.Cqrs.Abstractions;
using DevTrackr.Contracts;
using DevTrackr.SharedKernel.Primitives;
using GoalsService.Application.Abstractions.Persistence;
using GoalsService.Application.Goals.Commands;
using GoalsService.Domain.Goals;
using MassTransit;

namespace GoalsService.Application.Goals.Handlers;

public sealed class CancelGoalCommandHandler(
    IGoalRepository goalRepository,
    IUnitOfWork unitOfWork,
    IPublishEndpoint publishEndpoint)
    : ICommandHandler<CancelGoalCommand>
{
    public async Task<Result> HandleAsync(
        CancelGoalCommand command,
        CancellationToken cancellationToken = default)
    {
        var goal = await goalRepository.GetByIdAsync(command.GoalId, command.UserId, cancellationToken);
        if (goal is null)
        {
            return Result.Failure(GoalErrors.GoalNotFound);
        }

        var result = goal.Cancel(DateTime.UtcNow);
        if (result.IsFailure)
        {
            return Result.Failure(result.Error);
        }

        await publishEndpoint.Publish(
            new GoalCancelledIntegrationEvent(
                EventId: Guid.NewGuid(),
                GoalId: goal.Id,
                UserId: goal.UserId,
                OccurredAt: DateTime.UtcNow),
            cancellationToken);

        await unitOfWork.SaveChangesAsync(cancellationToken);
        return Result.Success();
    }
}
