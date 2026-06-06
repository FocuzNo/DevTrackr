using ActivityService.Application.Abstractions.Persistence;
using ActivityService.Application.Sessions.Commands;
using ActivityService.Application.Sessions.Responses;
using ActivityService.Domain.Sessions;
using DevTrackr.Contracts;
using DevTrackr.Cqrs.Abstractions;
using DevTrackr.SharedKernel.Primitives;
using MassTransit;

namespace ActivityService.Application.Sessions.Handlers;

public sealed class LogStudySessionCommandHandler(
    IStudySessionRepository studySessionRepository,
    IUnitOfWork unitOfWork,
    IPublishEndpoint publishEndpoint)
    : ICommandHandler<LogStudySessionCommand, Result<StudySessionResponse>>
{
    public async Task<Result<StudySessionResponse>> HandleAsync(LogStudySessionCommand command, CancellationToken cancellationToken = default)
    {
        var utcNow = DateTime.UtcNow;
        var studySessionResult = StudySession.Create(
            command.UserId,
            command.GoalId,
            command.Topic,
            command.DurationMinutes,
            command.Difficulty,
            command.Note,
            command.SessionDate,
            utcNow);

        if (studySessionResult.IsFailure || studySessionResult.Value is null)
        {
            return Result<StudySessionResponse>.Failure(studySessionResult.Error);
        }

        var studySession = studySessionResult.Value;

        await studySessionRepository.AddAsync(studySession, cancellationToken);

        await publishEndpoint.Publish(
            new StudySessionLoggedIntegrationEvent(
                EventId: Guid.NewGuid(),
                SessionId: studySession.Id,
                UserId: studySession.UserId,
                GoalId: studySession.GoalId,
                Topic: studySession.Topic,
                DurationMinutes: studySession.DurationMinutes,
                Difficulty: (int)studySession.Difficulty,
                SessionDate: studySession.SessionDate,
                OccurredAt: utcNow),
            cancellationToken);

        await unitOfWork.SaveChangesAsync(cancellationToken);

        return Result<StudySessionResponse>.Success(studySession.ToResponse());
    }
}
