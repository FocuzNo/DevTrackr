namespace StatisticsService.Application.Statistics.Responses;

public sealed record TopicStatisticsResponse(
    IReadOnlyList<TopicStatisticsItemResponse> Items);
