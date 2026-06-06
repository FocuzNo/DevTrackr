using ActivityService.Application.Sessions.Responses;

namespace ActivityService.Application.Abstractions.Persistence;

public interface IStudySessionReadRepository
{
    Task<StudySessionResponse?> GetByIdAsync(
        Guid sessionId,
        Guid userId,
        CancellationToken cancellationToken = default);

    Task<IReadOnlyList<StudySessionListItemResponse>> GetByDateRangeAsync(
        Guid userId,
        DateOnly from,
        DateOnly to,
        CancellationToken cancellationToken = default);

    Task<IReadOnlyList<StudySessionListItemResponse>> GetByGoalIdAsync(
        Guid userId,
        Guid goalId,
        CancellationToken cancellationToken = default);

    Task<IReadOnlyList<StudySessionListItemResponse>> GetByUserIdAsync(
        Guid userId,
        CancellationToken cancellationToken = default);
}
