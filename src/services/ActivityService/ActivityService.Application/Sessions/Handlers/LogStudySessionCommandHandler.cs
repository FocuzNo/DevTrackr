using ActivityService.Application.Abstractions;
using ActivityService.Application.Abstractions.Persistence;
using ActivityService.Application.Common;
using ActivityService.Application.Sessions.Commands;
using ActivityService.Application.Sessions.Responses;
using ActivityService.Domain.Sessions;
using DevTrackr.Contracts;
using DevTrackr.SharedKernel.Primitives;
using FluentValidation;
using MassTransit;

namespace ActivityService.Application.Sessions.Handlers;

public sealed class LogStudySessionCommandHandler(
    IStudySessionRepository studySessionRepository,
    IUnitOfWork unitOfWork,
    IValidator<LogStudySessionCommand> validator,
    IPublishEndpoint publishEndpoint)
    : ICommandHandler<LogStudySessionCommand, Result<StudySessionResponse>>
{
    public async Task<Result<StudySessionResponse>> HandleAsync(LogStudySessionCommand command, CancellationToken cancellationToken = default)
    {
        var validationResult = await validator.ValidateAsync(command, cancellationToken);
        if (!validationResult.IsValid)
        {
            return Result<StudySessionResponse>.Failure(validationResult.ToError());
        }

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
