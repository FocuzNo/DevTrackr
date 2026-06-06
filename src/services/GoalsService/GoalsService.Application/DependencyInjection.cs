using FluentValidation;
using GoalsService.Application.Abstractions;
using GoalsService.Application.Goals.Commands;
using GoalsService.Application.Goals.Handlers;
using GoalsService.Application.Goals.Queries;
using GoalsService.Application.Goals.Responses;
using Microsoft.Extensions.DependencyInjection;

namespace GoalsService.Application;

public static class DependencyInjection
{
    public static IServiceCollection AddApplication(this IServiceCollection services)
    {
        services.AddValidatorsFromAssembly(typeof(DependencyInjection).Assembly);

        services.AddScoped<ICommandHandler<CreateGoalCommand, DevTrackr.SharedKernel.Primitives.Result<GoalResponse>>, CreateGoalCommandHandler>();
        services.AddScoped<ICommandHandler<UpdateGoalCommand, DevTrackr.SharedKernel.Primitives.Result<GoalResponse>>, UpdateGoalCommandHandler>();
        services.AddScoped<ICommandHandler<CancelGoalCommand, DevTrackr.SharedKernel.Primitives.Result<GoalResponse>>, CancelGoalCommandHandler>();
        services.AddScoped<ICommandHandler<CompleteGoalCommand, DevTrackr.SharedKernel.Primitives.Result<GoalResponse>>, CompleteGoalCommandHandler>();
        services.AddScoped<ICommandHandler<AddGoalProgressCommand, DevTrackr.SharedKernel.Primitives.Result<GoalResponse>>, AddGoalProgressCommandHandler>();
        services.AddScoped<IQueryHandler<GetGoalsQuery, IReadOnlyList<GoalListItemResponse>>, GetGoalsQueryHandler>();
        services.AddScoped<IQueryHandler<GetGoalByIdQuery, DevTrackr.SharedKernel.Primitives.Result<GoalResponse>>, GetGoalByIdQueryHandler>();

        return services;
    }
}
