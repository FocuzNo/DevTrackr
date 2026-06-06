using DevTrackr.Cqrs.Abstractions;
using DevTrackr.SharedKernel.Primitives;
using StatisticsService.Application.Abstractions.Persistence;
using StatisticsService.Application.Statistics.Queries;
using StatisticsService.Application.Statistics.Responses;

namespace StatisticsService.Application.Statistics.Handlers;

public sealed class GetStreakQueryHandler(IStatisticsReadRepository repository)
    : IQueryHandler<GetStreakQuery, Result<StreakResponse>>
{
    public async Task<Result<StreakResponse>> HandleAsync(GetStreakQuery query, CancellationToken cancellationToken = default)
    {
        var userStatistics = await repository.GetUserStatisticsAsync(query.UserId, cancellationToken);

        return Result<StreakResponse>.Success(
            new StreakResponse(
                CurrentStreak: userStatistics?.CurrentStreak ?? 0,
                LongestStreak: userStatistics?.LongestStreak ?? 0,
                LastStudyDate: userStatistics?.LastStudyDate));
    }
}
