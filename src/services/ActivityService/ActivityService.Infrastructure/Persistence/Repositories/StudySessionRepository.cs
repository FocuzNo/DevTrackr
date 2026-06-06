using ActivityService.Application.Abstractions.Persistence;
using ActivityService.Domain.Sessions;
using Microsoft.EntityFrameworkCore;

namespace ActivityService.Infrastructure.Persistence.Repositories;

public sealed class StudySessionRepository(ActivityDbContext dbContext) : IStudySessionRepository
{
    public Task AddAsync(StudySession studySession, CancellationToken cancellationToken = default) =>
        dbContext.StudySessions.AddAsync(studySession, cancellationToken).AsTask();

    public Task<StudySession?> GetByIdAsync(Guid sessionId, Guid userId, CancellationToken cancellationToken = default) =>
        dbContext.StudySessions.FirstOrDefaultAsync(x => x.Id == sessionId && x.UserId == userId, cancellationToken);

    public async Task<IReadOnlyList<StudySession>> GetByUserIdAsync(Guid userId, CancellationToken cancellationToken = default) =>
        await dbContext.StudySessions
            .Where(x => x.UserId == userId)
            .OrderByDescending(x => x.SessionDate)
            .ThenByDescending(x => x.CreatedAt)
            .ToListAsync(cancellationToken);

    public async Task<IReadOnlyList<StudySession>> GetByGoalIdAsync(Guid userId, Guid goalId, CancellationToken cancellationToken = default) =>
        await dbContext.StudySessions
            .Where(x => x.UserId == userId && x.GoalId == goalId)
            .OrderByDescending(x => x.SessionDate)
            .ThenByDescending(x => x.CreatedAt)
            .ToListAsync(cancellationToken);

    public async Task<IReadOnlyList<StudySession>> GetByDateRangeAsync(
        Guid userId,
        DateOnly from,
        DateOnly to,
        CancellationToken cancellationToken = default) =>
        await dbContext.StudySessions
            .Where(x => x.UserId == userId && x.SessionDate >= from && x.SessionDate <= to)
            .OrderByDescending(x => x.SessionDate)
            .ThenByDescending(x => x.CreatedAt)
            .ToListAsync(cancellationToken);
}
