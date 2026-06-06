using DevTrackr.Cqrs.Abstractions;
using DevTrackr.SharedKernel.Primitives;
using GoalsService.Domain.Goals;
using GoalsService.Application.Goals.Responses;

namespace GoalsService.Application.Goals.Commands;

public sealed record CreateGoalCommand(
    Guid UserId,
    string Title,
    string? Description,
    GoalCategory Category,
    int TargetMinutes,
    DateOnly StartDate,
    DateOnly Deadline) : ICommand<Result<GoalResponse>>;
