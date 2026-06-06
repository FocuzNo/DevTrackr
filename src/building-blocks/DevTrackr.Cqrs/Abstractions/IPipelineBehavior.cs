namespace DevTrackr.Cqrs.Abstractions;

public delegate Task<TResult> RequestHandlerDelegate<TResult>();

public interface IPipelineBehavior<in TRequest, TResult>
{
    Task<TResult> HandleAsync(
        TRequest request,
        CancellationToken cancellationToken,
        RequestHandlerDelegate<TResult> next);
}
