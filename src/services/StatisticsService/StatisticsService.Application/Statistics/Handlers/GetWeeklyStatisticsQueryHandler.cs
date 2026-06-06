using DevTrackr.Cqrs.Abstractions;
using DevTrackr.SharedKernel.Primitives;
using StatisticsService.Application.Abstractions.Persistence;
using StatisticsService.Application.Statistics.Queries;
using StatisticsService.Application.Statistics.Responses;

namespace StatisticsService.Application.Statistics.Handlers;

public sealed class GetWeeklyStatisticsQueryHandler(IStatisticsReadRepository repository)
    : IQueryHandler<GetWeeklyStatisticsQuery, Result<WeeklyStatisticsResponse>>
{
    public async Task<Result<WeeklyStatisticsResponse>> HandleAsync(
        GetWeeklyStatisticsQuery query,
        CancellationToken cancellationToken = default)
    {
        var fromDate = DateOnly.FromDateTime(DateTime.UtcNow).AddDays(-6);
        var dailyStatistics = await repository.GetDailyStatisticsAsync(
            query.UserId,
            fromDate,
            DateOnly.FromDateTime(DateTime.UtcNow),
            cancellationToken);

        var days = Enumerable.Range(0, 7)
            .Select(offset => fromDate.AddDays(offset))
            .Select(date => dailyStatistics.FirstOrDefault(x => x.Date == date)?.ToResponse()
                ?? new DailyStatisticsItemResponse(date, 0, 0, 0))
            .ToArray();

        return Result<WeeklyStatisticsResponse>.Success(
            new WeeklyStatisticsResponse(
                Days: days,
                TotalMinutes: days.Sum(x => x.TotalMinutes),
                TotalSessions: days.Sum(x => x.SessionsCount)));
    }
}
