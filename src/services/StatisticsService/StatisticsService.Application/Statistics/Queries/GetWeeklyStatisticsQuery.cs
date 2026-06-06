using DevTrackr.Cqrs.Abstractions;
using DevTrackr.SharedKernel.Primitives;
using StatisticsService.Application.Statistics.Responses;

namespace StatisticsService.Application.Statistics.Queries;

public sealed record GetWeeklyStatisticsQuery(Guid UserId) : IQuery<Result<WeeklyStatisticsResponse>>;
