using DevTrackr.SharedKernel.Persistence;
using ActivityService.Domain.Sessions;

namespace ActivityService.Application.Abstractions.Persistence;

public interface IStudySessionRepository : IRepository<StudySession>
{
    Task<IReadOnlyList<StudySession>> GetByDateRangeAsync(
        Guid userId,
        DateOnly from,
        DateOnly to,
        CancellationToken cancellationToken = default);

    Task<StudySession?> GetByIdAsync(
        Guid sessionId,
        Guid userId,
        CancellationToken cancellationToken = default);

    Task<IReadOnlyList<StudySession>> GetByGoalIdAsync(
        Guid userId,
        Guid goalId,
        CancellationToken cancellationToken = default);

    Task<IReadOnlyList<StudySession>> GetByUserIdAsync(
        Guid userId,
        CancellationToken cancellationToken = default);
}
