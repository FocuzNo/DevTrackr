using DevTrackr.SharedKernel.Primitives;

namespace DevTrackr.Cqrs.Abstractions;

public interface IAppMediator
{
    Task<Result> SendAsync(ICommand command, CancellationToken cancellationToken = default);

    Task<TResult> SendAsync<TResult>(ICommand<TResult> command, CancellationToken cancellationToken = default);

    Task<TResult> SendAsync<TResult>(IQuery<TResult> query, CancellationToken cancellationToken = default);
}
