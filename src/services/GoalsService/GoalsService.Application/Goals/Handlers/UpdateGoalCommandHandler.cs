using DevTrackr.Cqrs.Abstractions;
using DevTrackr.SharedKernel.Primitives;
using GoalsService.Application.Abstractions.Persistence;
using GoalsService.Application.Goals.Commands;
using GoalsService.Application.Goals.Responses;
using GoalsService.Domain.Goals;

namespace GoalsService.Application.Goals.Handlers;

public sealed class UpdateGoalCommandHandler(
    IGoalRepository goalRepository,
    IUnitOfWork unitOfWork)
    : ICommandHandler<UpdateGoalCommand, Result<GoalResponse>>
{
    public async Task<Result<GoalResponse>> HandleAsync(UpdateGoalCommand command, CancellationToken cancellationToken = default)
    {
        var goal = await goalRepository.GetByIdAsync(command.GoalId, command.UserId, cancellationToken);
        if (goal is null)
        {
            return Result<GoalResponse>.Failure(GoalErrors.GoalNotFound);
        }

        var updateResult = goal.UpdateDetails(
            command.Title,
            command.Description,
            command.Category,
            command.TargetMinutes,
            command.StartDate,
            command.Deadline,
            DateTime.UtcNow);

        if (updateResult.IsFailure)
        {
            return Result<GoalResponse>.Failure(updateResult.Error);
        }

        await unitOfWork.SaveChangesAsync(cancellationToken);
        return Result<GoalResponse>.Success(goal.ToResponse());
    }
}
