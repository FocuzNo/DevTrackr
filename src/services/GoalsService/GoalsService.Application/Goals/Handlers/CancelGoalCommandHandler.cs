using DevTrackr.SharedKernel.Primitives;
using GoalsService.Application.Abstractions;
using GoalsService.Application.Abstractions.Persistence;
using GoalsService.Application.Goals.Commands;
using GoalsService.Application.Goals.Responses;
using GoalsService.Domain.Goals;

namespace GoalsService.Application.Goals.Handlers;

public sealed class CancelGoalCommandHandler(
    IGoalRepository goalRepository,
    IUnitOfWork unitOfWork)
    : ICommandHandler<CancelGoalCommand, Result<GoalResponse>>
{
    public async Task<Result<GoalResponse>> HandleAsync(CancelGoalCommand command, CancellationToken cancellationToken = default)
    {
        var goal = await goalRepository.GetByIdAsync(command.GoalId, command.UserId, cancellationToken);
        if (goal is null)
        {
            return Result<GoalResponse>.Failure(GoalErrors.GoalNotFound);
        }

        var result = goal.Cancel(DateTime.UtcNow);
        if (result.IsFailure)
        {
            return Result<GoalResponse>.Failure(result.Error);
        }

        await unitOfWork.SaveChangesAsync(cancellationToken);
        return Result<GoalResponse>.Success(goal.ToResponse());
    }
}
