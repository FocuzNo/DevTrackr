using DevTrackr.Contracts;
using DevTrackr.SharedKernel.Primitives;
using FluentValidation;
using GoalsService.Application.Abstractions;
using GoalsService.Application.Abstractions.Persistence;
using GoalsService.Application.Common;
using GoalsService.Application.Goals.Commands;
using GoalsService.Application.Goals.Responses;
using GoalsService.Domain.Goals;
using MassTransit;

namespace GoalsService.Application.Goals.Handlers;

public sealed class AddGoalProgressCommandHandler(
    IGoalRepository goalRepository,
    IUnitOfWork unitOfWork,
    IPublishEndpoint publishEndpoint,
    IValidator<AddGoalProgressCommand> validator)
    : ICommandHandler<AddGoalProgressCommand, Result<GoalResponse>>
{
    public async Task<Result<GoalResponse>> HandleAsync(AddGoalProgressCommand command, CancellationToken cancellationToken = default)
    {
        var validationResult = await validator.ValidateAsync(command, cancellationToken);
        if (!validationResult.IsValid)
        {
            return Result<GoalResponse>.Failure(validationResult.ToError());
        }

        var goal = await goalRepository.GetByIdAsync(command.GoalId, command.UserId, cancellationToken);
        if (goal is null)
        {
            return Result<GoalResponse>.Failure(GoalErrors.GoalNotFound);
        }

        var utcNow = DateTime.UtcNow;
        var result = goal.AddProgress(command.MinutesToAdd, utcNow);
        if (result.IsFailure)
        {
            return Result<GoalResponse>.Failure(result.Error);
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

        await unitOfWork.SaveChangesAsync(cancellationToken);
        return Result<GoalResponse>.Success(goal.ToResponse());
    }
}
