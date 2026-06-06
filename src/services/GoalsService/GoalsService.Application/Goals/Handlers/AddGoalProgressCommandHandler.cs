using DevTrackr.Cqrs.Abstractions;
using DevTrackr.Contracts;
using DevTrackr.SharedKernel.Primitives;
using GoalsService.Application.Abstractions.Persistence;
using GoalsService.Application.Goals.Commands;
using GoalsService.Domain.Goals;
using MassTransit;

namespace GoalsService.Application.Goals.Handlers;

public sealed class AddGoalProgressCommandHandler(
    IGoalRepository goalRepository,
    IPublishEndpoint publishEndpoint)
    : ICommandHandler<AddGoalProgressCommand>
{
    public async Task<Result> HandleAsync(AddGoalProgressCommand command, CancellationToken cancellationToken = default)
    {
        var goal = await goalRepository.GetByIdAsync(command.GoalId, command.UserId, cancellationToken);
        if (goal is null)
        {
            return Result.Failure(GoalErrors.GoalNotFound);
        }

        var utcNow = DateTime.UtcNow;
        var result = goal.AddProgress(command.MinutesToAdd, utcNow);
        if (result.IsFailure)
        {
            return Result.Failure(result.Error);
        }

        await publishEndpoint.Publish(
            new GoalProgressUpdatedIntegrationEvent(
                EventId: Guid.NewGuid(),
                GoalId: goal.Id,
                UserId: goal.UserId,
                CurrentMinutes: goal.CurrentMinutes,
                TargetMinutes: goal.TargetMinutes,
                ProgressPercentage: goal.ProgressPercentage,
                OccurredOnUtc: utcNow),
            cancellationToken);

        return Result.Success();
    }
}
