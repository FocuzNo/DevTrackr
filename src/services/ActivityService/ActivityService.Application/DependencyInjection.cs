using FluentValidation;
using ActivityService.Application.Abstractions;
using ActivityService.Application.Sessions.Commands;
using ActivityService.Application.Sessions.Handlers;
using ActivityService.Application.Sessions.Queries;
using ActivityService.Application.Sessions.Responses;
using Microsoft.Extensions.DependencyInjection;

namespace ActivityService.Application;

public static class DependencyInjection
{
    public static IServiceCollection AddApplication(this IServiceCollection services)
    {
        services.AddValidatorsFromAssembly(typeof(DependencyInjection).Assembly);
        services.AddScoped<ICommandHandler<LogStudySessionCommand, DevTrackr.SharedKernel.Primitives.Result<StudySessionResponse>>, LogStudySessionCommandHandler>();
        services.AddScoped<IQueryHandler<GetStudySessionByIdQuery, DevTrackr.SharedKernel.Primitives.Result<StudySessionResponse>>, GetStudySessionByIdQueryHandler>();
        services.AddScoped<IQueryHandler<GetStudySessionsQuery, IReadOnlyList<StudySessionListItemResponse>>, GetStudySessionsQueryHandler>();
        services.AddScoped<IQueryHandler<GetStudySessionsByGoalQuery, IReadOnlyList<StudySessionListItemResponse>>, GetStudySessionsByGoalQueryHandler>();
        services.AddScoped<IQueryHandler<GetStudySessionsByDateRangeQuery, DevTrackr.SharedKernel.Primitives.Result<IReadOnlyList<StudySessionListItemResponse>>>, GetStudySessionsByDateRangeQueryHandler>();
        return services;
    }
}
