using DevTrackr.Cqrs.Abstractions;
using DevTrackr.SharedKernel.Primitives;
using StatisticsService.Application.Statistics.Responses;

namespace StatisticsService.Application.Statistics.Queries;

public sealed record GetTopicStatisticsQuery(Guid UserId) : IQuery<Result<TopicStatisticsResponse>>;
