using DevTrackr.Contracts;
using DevTrackr.SharedKernel.Primitives;
using GoalsService.Application.Abstractions;
using GoalsService.Application.Abstractions.Persistence;
using GoalsService.Application.Goals.Commands;
using GoalsService.Application.Goals.Responses;
using GoalsService.Domain.Goals;
using MassTransit;

namespace GoalsService.Application.Goals.Handlers;

public sealed class CompleteGoalCommandHandler(
    IGoalRepository goalRepository,
    IUnitOfWork unitOfWork,
    IPublishEndpoint publishEndpoint)
    : ICommandHandler<CompleteGoalCommand, Result<GoalResponse>>
{
    public async Task<Result<GoalResponse>> HandleAsync(CompleteGoalCommand command, CancellationToken cancellationToken = default)
    {
        var goal = await goalRepository.GetByIdAsync(command.GoalId, command.UserId, cancellationToken);
        if (goal is null)
        {
            return Result<GoalResponse>.Failure(GoalErrors.GoalNotFound);
        }

        var utcNow = DateTime.UtcNow;
        var result = goal.Complete(utcNow);
        if (result.IsFailure)
        {
            return Result<GoalResponse>.Failure(result.Error);
        }

        await publishEndpoint.Publish(
            new GoalCompletedIntegrationEvent(
                EventId: Guid.NewGuid(),
                GoalId: goal.Id,
                UserId: goal.UserId,
                CurrentMinutes: goal.CurrentMinutes,
                CompletedAtUtc: goal.CompletedAt ?? utcNow,
                OccurredOnUtc: utcNow),
            cancellationToken);

        await unitOfWork.SaveChangesAsync(cancellationToken);
        return Result<GoalResponse>.Success(goal.ToResponse());
    }
}
