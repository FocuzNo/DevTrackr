using DevTrackr.Contracts;
using Microsoft.Extensions.Logging;
using StatisticsService.Application.Abstractions.Caching;
using StatisticsService.Domain.Statistics;
using StatisticsService.Infrastructure.Persistence;
using StatisticsService.Infrastructure.Persistence.Entities;
using StatisticsService.Infrastructure.Persistence.Repositories;

namespace StatisticsService.Infrastructure.Messaging;

public sealed class StudySessionLoggedEventProcessor(
    IStatisticsProjectionRepository repository,
    IProcessedIntegrationEventRepository processedEvents,
    IStatisticsUnitOfWork unitOfWork,
    IDashboardCache cache,
    ILogger<StudySessionLoggedEventProcessor> logger)
{
    public async Task ProcessAsync(StudySessionLoggedIntegrationEvent integrationEvent, CancellationToken cancellationToken)
    {
        if (await processedEvents.ExistsAsync(integrationEvent.EventId, cancellationToken))
        {
            logger.LogInformation(
                "Study session event {EventId} already processed for UserId {UserId}",
                integrationEvent.EventId,
                integrationEvent.UserId);
            return;
        }

        var updatedAtUtc = integrationEvent.OccurredAt;
        var dailyStatistics = await repository.GetDailyStatisticsAsync(
            integrationEvent.UserId,
            integrationEvent.SessionDate,
            cancellationToken)
            ?? DailyStatistics.Create(integrationEvent.UserId, integrationEvent.SessionDate, updatedAtUtc);

        if (dailyStatistics.Id == Guid.Empty)
        {
            throw new InvalidOperationException("Daily statistics entity was not initialized correctly.");
        }

        var isNewDailyStatistics = dailyStatistics.SessionsCount == 0;
        if (isNewDailyStatistics)
        {
            await repository.AddAsync(dailyStatistics, cancellationToken);
        }

        dailyStatistics.ApplyStudySession(integrationEvent.DurationMinutes, integrationEvent.Difficulty, updatedAtUtc);

        var topicStatistics = await repository.GetTopicStatisticsAsync(
            integrationEvent.UserId,
            integrationEvent.Topic,
            cancellationToken)
            ?? TopicStatistics.Create(integrationEvent.UserId, integrationEvent.Topic, updatedAtUtc);

        if (topicStatistics.SessionsCount == 0)
        {
            await repository.AddAsync(topicStatistics, cancellationToken);
        }

        topicStatistics.ApplyStudySession(integrationEvent.DurationMinutes, integrationEvent.Difficulty, updatedAtUtc);

        var userStatistics = await repository.GetUserStatisticsAsync(integrationEvent.UserId, cancellationToken)
            ?? UserStatistics.Create(integrationEvent.UserId, updatedAtUtc);

        if (userStatistics.TotalSessions == 0 && userStatistics.TotalStudyMinutes == 0 && userStatistics.Id != Guid.Empty)
        {
            var existing = await repository.GetUserStatisticsAsync(integrationEvent.UserId, cancellationToken);
            if (existing is null)
            {
                await repository.AddAsync(userStatistics, cancellationToken);
            }
            else
            {
                userStatistics = existing;
            }
        }

        var studyDates = await repository.GetStudyDatesAsync(integrationEvent.UserId, cancellationToken);
        var recalculated = RecalculateStreaks(studyDates, integrationEvent.SessionDate);

        userStatistics.ApplyStudySession(
            integrationEvent.DurationMinutes,
            integrationEvent.Difficulty,
            integrationEvent.SessionDate,
            recalculated.currentStreak,
            recalculated.longestStreak,
            updatedAtUtc);

        await processedEvents.AddAsync(
            new ProcessedIntegrationEvent
            {
                EventId = integrationEvent.EventId,
                EventType = nameof(StudySessionLoggedIntegrationEvent),
                ProcessedAtUtc = DateTime.UtcNow
            },
            cancellationToken);

        await unitOfWork.SaveChangesAsync(cancellationToken);
        await cache.InvalidateAsync(integrationEvent.UserId, cancellationToken);
    }

    public static (int currentStreak, int longestStreak) RecalculateStreaks(
        IReadOnlyList<DateOnly> orderedDescendingDates,
        DateOnly newDate)
    {
        var dates = orderedDescendingDates
            .Append(newDate)
            .Distinct()
            .OrderByDescending(x => x)
            .ToArray();

        if (dates.Length == 0)
        {
            return (0, 0);
        }

        var currentStreak = 1;
        for (var i = 1; i < dates.Length; i++)
        {
            if (dates[i - 1].AddDays(-1) != dates[i])
            {
                break;
            }

            currentStreak++;
        }

        var longestStreak = 1;
        var streak = 1;
        for (var i = 1; i < dates.Length; i++)
        {
            if (dates[i - 1].AddDays(-1) == dates[i])
            {
                streak++;
                longestStreak = Math.Max(longestStreak, streak);
                continue;
            }

            streak = 1;
        }

        return (currentStreak, longestStreak);
    }
}
