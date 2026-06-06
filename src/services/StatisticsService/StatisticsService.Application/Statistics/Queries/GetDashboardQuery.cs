using DevTrackr.Cqrs.Abstractions;
using DevTrackr.SharedKernel.Primitives;
using StatisticsService.Application.Statistics.Responses;

namespace StatisticsService.Application.Statistics.Queries;

public sealed record GetDashboardQuery(Guid UserId) : IQuery<Result<DashboardResponse>>;
