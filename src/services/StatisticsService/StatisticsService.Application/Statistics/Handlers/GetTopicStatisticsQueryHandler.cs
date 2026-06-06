using DevTrackr.Cqrs.Abstractions;
using DevTrackr.SharedKernel.Primitives;
using StatisticsService.Application.Abstractions.Persistence;
using StatisticsService.Application.Statistics.Queries;
using StatisticsService.Application.Statistics.Responses;

namespace StatisticsService.Application.Statistics.Handlers;

public sealed class GetTopicStatisticsQueryHandler(IStatisticsReadRepository repository)
    : IQueryHandler<GetTopicStatisticsQuery, Result<TopicStatisticsResponse>>
{
    public async Task<Result<TopicStatisticsResponse>> HandleAsync(
        GetTopicStatisticsQuery query,
        CancellationToken cancellationToken = default)
    {
        var topics = await repository.GetTopicStatisticsAsync(query.UserId, cancellationToken);

        return Result<TopicStatisticsResponse>.Success(
            new TopicStatisticsResponse(
                topics
                    .OrderByDescending(x => x.TotalMinutes)
                    .ThenBy(x => x.Topic)
                    .Select(x => x.ToResponse())
                    .ToArray()));
    }
}
