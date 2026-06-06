using ActivityService.Application.Abstractions;
using ActivityService.Application.Abstractions.Persistence;
using ActivityService.Application.Common;
using ActivityService.Application.Sessions.Queries;
using ActivityService.Application.Sessions.Responses;
using DevTrackr.SharedKernel.Primitives;
using FluentValidation;

namespace ActivityService.Application.Sessions.Handlers;

public sealed class GetStudySessionsByDateRangeQueryHandler(
    IStudySessionRepository studySessionRepository,
    IValidator<GetStudySessionsByDateRangeQuery> validator)
    : IQueryHandler<GetStudySessionsByDateRangeQuery, Result<IReadOnlyList<StudySessionListItemResponse>>>
{
    public async Task<Result<IReadOnlyList<StudySessionListItemResponse>>> HandleAsync(
        GetStudySessionsByDateRangeQuery query,
        CancellationToken cancellationToken = default)
    {
        var validationResult = await validator.ValidateAsync(query, cancellationToken);
        if (!validationResult.IsValid)
        {
            return Result<IReadOnlyList<StudySessionListItemResponse>>.Failure(validationResult.ToError());
        }

        var studySessions = await studySessionRepository.GetByDateRangeAsync(query.UserId, query.From, query.To, cancellationToken);
        var response = studySessions.Select(x => x.ToListItemResponse()).ToArray();
        return Result<IReadOnlyList<StudySessionListItemResponse>>.Success(response);
    }
}
