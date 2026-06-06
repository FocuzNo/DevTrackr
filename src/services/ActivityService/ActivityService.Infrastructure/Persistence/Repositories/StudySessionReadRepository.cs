using ActivityService.Application.Abstractions.Persistence;
using ActivityService.Application.Sessions.Responses;
using Microsoft.EntityFrameworkCore;

namespace ActivityService.Infrastructure.Persistence.Repositories;

public sealed class StudySessionReadRepository(ActivityDbContext dbContext) : IStudySessionReadRepository
{
    public Task<StudySessionResponse?> GetByIdAsync(
        Guid sessionId,
        Guid userId,
        CancellationToken cancellationToken = default) =>
        dbContext.StudySessions
            .AsNoTracking()
            .Where(x => x.Id == sessionId && x.UserId == userId)
            .Select(x => new StudySessionResponse(
                x.Id,
                x.UserId,
                x.GoalId,
                x.Topic,
                x.DurationMinutes,
                x.Difficulty,
                x.Note,
                x.SessionDate,
                x.CreatedAt,
                x.UpdatedAt))
            .FirstOrDefaultAsync(cancellationToken);

    public async Task<IReadOnlyList<StudySessionListItemResponse>> GetByDateRangeAsync(
        Guid userId,
        DateOnly from,
        DateOnly to,
        CancellationToken cancellationToken = default) =>
        await dbContext.StudySessions
            .AsNoTracking()
            .Where(x => x.UserId == userId && x.SessionDate >= from && x.SessionDate <= to)
            .OrderByDescending(x => x.SessionDate)
            .ThenByDescending(x => x.CreatedAt)
            .Select(x => new StudySessionListItemResponse(
                x.Id,
                x.GoalId,
                x.Topic,
                x.DurationMinutes,
                x.Difficulty,
                x.SessionDate,
                x.UpdatedAt))
            .ToListAsync(cancellationToken);

    public async Task<IReadOnlyList<StudySessionListItemResponse>> GetByGoalIdAsync(
        Guid userId,
        Guid goalId,
        CancellationToken cancellationToken = default) =>
        await dbContext.StudySessions
            .AsNoTracking()
            .Where(x => x.UserId == userId && x.GoalId == goalId)
            .OrderByDescending(x => x.SessionDate)
            .ThenByDescending(x => x.CreatedAt)
            .Select(x => new StudySessionListItemResponse(
                x.Id,
                x.GoalId,
                x.Topic,
                x.DurationMinutes,
                x.Difficulty,
                x.SessionDate,
                x.UpdatedAt))
            .ToListAsync(cancellationToken);

    public async Task<IReadOnlyList<StudySessionListItemResponse>> GetByUserIdAsync(
        Guid userId,
        CancellationToken cancellationToken = default) =>
        await dbContext.StudySessions
            .AsNoTracking()
            .Where(x => x.UserId == userId)
            .OrderByDescending(x => x.SessionDate)
            .ThenByDescending(x => x.CreatedAt)
            .Select(x => new StudySessionListItemResponse(
                x.Id,
                x.GoalId,
                x.Topic,
                x.DurationMinutes,
                x.Difficulty,
                x.SessionDate,
                x.UpdatedAt))
            .ToListAsync(cancellationToken);
}
