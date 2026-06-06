using DevTrackr.Cqrs.Abstractions;
using DevTrackr.SharedKernel.Primitives;
using Microsoft.Extensions.DependencyInjection;
using System.Reflection;
using System.Runtime.ExceptionServices;

namespace DevTrackr.Cqrs;

public sealed class AppMediator(IServiceProvider serviceProvider) : IAppMediator
{
    private static readonly MethodInfo SendCommandMethod = typeof(AppMediator)
        .GetMethod(nameof(SendCommandInternalAsync), BindingFlags.Instance | BindingFlags.NonPublic)
        ?? throw new InvalidOperationException("SendCommandInternalAsync method was not found.");

    private static readonly MethodInfo SendCommandWithResultMethod = typeof(AppMediator)
        .GetMethod(nameof(SendCommandWithResultInternalAsync), BindingFlags.Instance | BindingFlags.NonPublic)
        ?? throw new InvalidOperationException("SendCommandWithResultInternalAsync method was not found.");

    private static readonly MethodInfo SendQueryMethod = typeof(AppMediator)
        .GetMethod(nameof(SendQueryInternalAsync), BindingFlags.Instance | BindingFlags.NonPublic)
        ?? throw new InvalidOperationException("SendQueryInternalAsync method was not found.");

    public Task<Result> SendAsync(ICommand command, CancellationToken cancellationToken = default)
    {
        ArgumentNullException.ThrowIfNull(command);

        var method = SendCommandMethod.MakeGenericMethod(command.GetType());
        return InvokeMethod<Task<Result>>(method, [command, cancellationToken], command.GetType().Name, "command");
    }

    public Task<TResult> SendAsync<TResult>(ICommand<TResult> command, CancellationToken cancellationToken = default)
    {
        ArgumentNullException.ThrowIfNull(command);

        var method = SendCommandWithResultMethod.MakeGenericMethod(command.GetType(), typeof(TResult));
        return InvokeMethod<Task<TResult>>(method, [command, cancellationToken], command.GetType().Name, "command");
    }

    public Task<TResult> SendAsync<TResult>(IQuery<TResult> query, CancellationToken cancellationToken = default)
    {
        ArgumentNullException.ThrowIfNull(query);

        var method = SendQueryMethod.MakeGenericMethod(query.GetType(), typeof(TResult));
        return InvokeMethod<Task<TResult>>(method, [query, cancellationToken], query.GetType().Name, "query");
    }

    private Task<Result> SendCommandInternalAsync<TCommand>(TCommand command, CancellationToken cancellationToken)
        where TCommand : ICommand
    {
        var handler = serviceProvider.GetService<ICommandHandler<TCommand>>()
            ?? throw CreateMissingHandlerException(typeof(TCommand), typeof(ICommandHandler<TCommand>));

        return ExecutePipelineAsync<TCommand, Result>(
            command,
            cancellationToken,
            token => handler.HandleAsync(command, token));
    }

    private Task<TResult> SendCommandWithResultInternalAsync<TCommand, TResult>(TCommand command, CancellationToken cancellationToken)
        where TCommand : ICommand<TResult>
    {
        var handler = serviceProvider.GetService<ICommandHandler<TCommand, TResult>>()
            ?? throw CreateMissingHandlerException(typeof(TCommand), typeof(ICommandHandler<TCommand, TResult>));

        return ExecutePipelineAsync<TCommand, TResult>(
            command,
            cancellationToken,
            token => handler.HandleAsync(command, token));
    }

    private Task<TResult> SendQueryInternalAsync<TQuery, TResult>(TQuery query, CancellationToken cancellationToken)
        where TQuery : IQuery<TResult>
    {
        var handler = serviceProvider.GetService<IQueryHandler<TQuery, TResult>>()
            ?? throw CreateMissingHandlerException(typeof(TQuery), typeof(IQueryHandler<TQuery, TResult>));

        return ExecutePipelineAsync<TQuery, TResult>(
            query,
            cancellationToken,
            token => handler.HandleAsync(query, token));
    }

    private async Task<TResult> ExecutePipelineAsync<TRequest, TResult>(
        TRequest request,
        CancellationToken cancellationToken,
        Func<CancellationToken, Task<TResult>> handlerInvoker)
    {
        var behaviors = serviceProvider.GetServices<IPipelineBehavior<TRequest, TResult>>().Reverse().ToArray();

        RequestHandlerDelegate<TResult> next = () => handlerInvoker(cancellationToken);

        foreach (var behavior in behaviors)
        {
            var currentBehavior = behavior;
            var currentNext = next;
            next = () => currentBehavior.HandleAsync(request, cancellationToken, currentNext);
        }

        return await next();
    }

    private static InvalidOperationException CreateMissingHandlerException(Type requestType, Type handlerType) =>
        new($"No handler was registered for request '{requestType.FullName}'. Expected handler: '{handlerType.FullName}'.");

    private TReturn InvokeMethod<TReturn>(MethodInfo method, object?[] arguments, string requestName, string requestKind)
    {
        try
        {
            return (TReturn)(method.Invoke(this, arguments)
                ?? throw new InvalidOperationException($"Failed to execute {requestKind} {requestName}."));
        }
        catch (TargetInvocationException exception) when (exception.InnerException is not null)
        {
            ExceptionDispatchInfo.Capture(exception.InnerException).Throw();
            throw;
        }
    }
}
