using DevTrackr.Cqrs.Abstractions;
using DevTrackr.SharedKernel.Primitives;
using GoalsService.Application.Abstractions.Persistence;
using GoalsService.Application.Goals.Commands;
using GoalsService.Application.Goals.Responses;
using GoalsService.Domain.Goals;

namespace GoalsService.Application.Goals.Handlers;

public sealed class CreateGoalCommandHandler(
    IGoalRepository goalRepository,
    IUnitOfWork unitOfWork)
    : ICommandHandler<CreateGoalCommand, Result<GoalResponse>>
{
    public async Task<Result<GoalResponse>> HandleAsync(CreateGoalCommand command, CancellationToken cancellationToken = default)
    {
        var goalResult = Goal.Create(
            command.UserId,
            command.Title,
            command.Description,
            command.Category,
            command.TargetMinutes,
            command.StartDate,
            command.Deadline,
            DateTime.UtcNow);

        if (goalResult.IsFailure || goalResult.Value is null)
        {
            return Result<GoalResponse>.Failure(goalResult.Error);
        }

        await goalRepository.AddAsync(goalResult.Value, cancellationToken);
        await unitOfWork.SaveChangesAsync(cancellationToken);

        return Result<GoalResponse>.Success(goalResult.Value.ToResponse());
    }
}
