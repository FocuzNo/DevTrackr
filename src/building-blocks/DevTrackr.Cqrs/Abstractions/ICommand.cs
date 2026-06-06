namespace DevTrackr.Cqrs.Abstractions;

public interface ICommand
{
}

public interface ICommand<out TResult>
{
}
