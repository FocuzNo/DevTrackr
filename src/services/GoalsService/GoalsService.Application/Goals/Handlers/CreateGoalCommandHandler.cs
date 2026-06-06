using DevTrackr.Cqrs.Abstractions;
using DevTrackr.Contracts;
using DevTrackr.SharedKernel.Primitives;
using GoalsService.Application.Abstractions.Persistence;
using GoalsService.Application.Goals.Commands;
using GoalsService.Application.Goals.Responses;
using GoalsService.Domain.Goals;
using MassTransit;

namespace GoalsService.Application.Goals.Handlers;

public sealed class CreateGoalCommandHandler(
    IGoalRepository goalRepository,
    IUnitOfWork unitOfWork,
    IPublishEndpoint publishEndpoint)
    : ICommandHandler<CreateGoalCommand, Result<GoalResponse>>
{
    public async Task<Result<GoalResponse>> HandleAsync(
        CreateGoalCommand command,
        CancellationToken cancellationToken = default)
    {
        var utcNow = DateTime.UtcNow;
        var goalResult = Goal.Create(
            command.UserId,
            command.Title,
            command.Description,
            command.Category,
            command.TargetMinutes,
            command.StartDate,
            command.Deadline,
            utcNow);

        if (goalResult.IsFailure || goalResult.Value is null)
        {
            return Result<GoalResponse>.Failure(goalResult.Error);
        }

        await goalRepository.AddAsync(goalResult.Value, cancellationToken);
        await publishEndpoint.Publish(
            new GoalCreatedIntegrationEvent(
                EventId: Guid.NewGuid(),
                GoalId: goalResult.Value.Id,
                UserId: goalResult.Value.UserId,
                Title: goalResult.Value.Title,
                Category: goalResult.Value.Category.ToString(),
                TargetMinutes: goalResult.Value.TargetMinutes,
                OccurredAt: utcNow),
            cancellationToken);
        await unitOfWork.SaveChangesAsync(cancellationToken);

        return Result<GoalResponse>.Success(goalResult.Value.ToResponse());
    }
}
