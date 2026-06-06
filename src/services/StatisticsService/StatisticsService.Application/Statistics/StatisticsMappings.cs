using StatisticsService.Application.Statistics.Responses;
using StatisticsService.Domain.Statistics;

namespace StatisticsService.Application.Statistics;

internal static class StatisticsMappings
{
    public static TopicStatisticsItemResponse ToResponse(this TopicStatistics topic) =>
        new(topic.Topic, topic.TotalMinutes, topic.SessionsCount, topic.AverageDifficulty);

    public static TopicSummaryItemResponse ToSummaryResponse(this TopicStatistics topic) =>
        new(topic.Topic, topic.TotalMinutes);

    public static DailyStatisticsItemResponse ToResponse(this DailyStatistics daily) =>
        new(daily.Date, daily.TotalMinutes, daily.SessionsCount, daily.AverageDifficulty);
}
