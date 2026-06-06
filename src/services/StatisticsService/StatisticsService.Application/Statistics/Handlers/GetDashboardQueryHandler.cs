using DevTrackr.Cqrs.Abstractions;
using DevTrackr.SharedKernel.Primitives;
using StatisticsService.Application.Abstractions.Caching;
using StatisticsService.Application.Abstractions.Persistence;
using StatisticsService.Application.Statistics.Queries;
using StatisticsService.Application.Statistics.Responses;

namespace StatisticsService.Application.Statistics.Handlers;

public sealed class GetDashboardQueryHandler(
    IStatisticsReadRepository repository,
    IDashboardCache cache)
    : IQueryHandler<GetDashboardQuery, Result<DashboardResponse>>
{
    public async Task<Result<DashboardResponse>> HandleAsync(GetDashboardQuery query, CancellationToken cancellationToken = default)
    {
        var cached = await cache.GetAsync(query.UserId, cancellationToken);
        if (cached is not null)
        {
            return Result<DashboardResponse>.Success(cached);
        }

        var response = await BuildDashboardAsync(query.UserId, cancellationToken);
        await cache.SetAsync(query.UserId, response, cancellationToken);

        return Result<DashboardResponse>.Success(response);
    }

    private async Task<DashboardResponse> BuildDashboardAsync(Guid userId, CancellationToken cancellationToken)
    {
        var userStatistics = await repository.GetUserStatisticsAsync(userId, cancellationToken);
        var topicStatistics = await repository.GetTopicStatisticsAsync(userId, cancellationToken);
        var fromDate = DateOnly.FromDateTime(DateTime.UtcNow).AddDays(-6);
        var dailyStatistics = await repository.GetDailyStatisticsAsync(
            userId,
            fromDate,
            DateOnly.FromDateTime(DateTime.UtcNow),
            cancellationToken);

        return new DashboardResponse(
            TotalStudyMinutes: userStatistics?.TotalStudyMinutes ?? 0,
            TotalSessions: userStatistics?.TotalSessions ?? 0,
            CompletedGoals: userStatistics?.CompletedGoals ?? 0,
            ActiveGoals: userStatistics?.ActiveGoals ?? 0,
            CurrentStreak: userStatistics?.CurrentStreak ?? 0,
            LongestStreak: userStatistics?.LongestStreak ?? 0,
            AverageDifficulty: userStatistics?.AverageDifficulty ?? 0,
            TopTopics: topicStatistics
                .OrderByDescending(x => x.TotalMinutes)
                .ThenBy(x => x.Topic)
                .Take(5)
                .Select(x => x.ToSummaryResponse())
                .ToArray(),
            WeeklyMinutes: Enumerable.Range(0, 7)
                .Select(offset => fromDate.AddDays(offset))
                .Select(date => dailyStatistics.FirstOrDefault(x => x.Date == date)?.ToResponse()
                    ?? new DailyStatisticsItemResponse(date, 0, 0, 0))
                .ToArray());
    }
}
