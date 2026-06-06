using DevTrackr.Cqrs.Abstractions;
using DevTrackr.Cqrs.DependencyInjection;
using DevTrackr.SharedKernel.Primitives;
using FluentValidation;
using Microsoft.Extensions.DependencyInjection;
using Xunit;

namespace DevTrackr.Cqrs.UnitTests;

public sealed class AppMediatorTests
{
    [Fact]
    public async Task SendAsync_ExecutesCommandHandler()
    {
        var serviceProvider = CreateServiceProvider();
        var mediator = serviceProvider.GetRequiredService<IAppMediator>();

        var result = await mediator.SendAsync(new CreateSampleCommand("DevTrackr"));

        Assert.True(result.IsSuccess);
        Assert.Equal("Created: DevTrackr", result.Value);
    }

    [Fact]
    public async Task SendAsync_ExecutesQueryHandler()
    {
        var serviceProvider = CreateServiceProvider();
        var mediator = serviceProvider.GetRequiredService<IAppMediator>();

        var result = await mediator.SendAsync(new CountSampleQuery(7));

        Assert.Equal(14, result);
    }

    [Fact]
    public async Task SendAsync_MissingHandler_ThrowsClearException()
    {
        var serviceProvider = CreateServiceProvider();
        var mediator = serviceProvider.GetRequiredService<IAppMediator>();

        var exception = await Assert.ThrowsAsync<InvalidOperationException>(() =>
            mediator.SendAsync(new MissingHandlerCommand()));

        Assert.Contains(nameof(MissingHandlerCommand), exception.Message);
        Assert.Contains(nameof(ICommandHandler<MissingHandlerCommand>), exception.Message);
    }

    [Fact]
    public async Task SendAsync_InvalidCommand_ReturnsValidationFailureResult()
    {
        var serviceProvider = CreateServiceProvider();
        var mediator = serviceProvider.GetRequiredService<IAppMediator>();

        var result = await mediator.SendAsync(new CreateSampleCommand(string.Empty));

        Assert.True(result.IsFailure);
        Assert.Equal("Validation", result.Error.Code);
        Assert.Contains("Name", result.Error.Message);
    }

    private static IServiceProvider CreateServiceProvider()
    {
        var services = new ServiceCollection();
        services.AddCqrs(typeof(AppMediatorTests).Assembly);
        services.AddScoped<IValidator<CreateSampleCommand>, CreateSampleCommandValidator>();
        return services.BuildServiceProvider();
    }

    private sealed record CreateSampleCommand(string Name) : ICommand<Result<string>>;

    private sealed class CreateSampleCommandHandler : ICommandHandler<CreateSampleCommand, Result<string>>
    {
        public Task<Result<string>> HandleAsync(CreateSampleCommand command, CancellationToken cancellationToken = default) =>
            Task.FromResult(Result<string>.Success($"Created: {command.Name}"));
    }

    private sealed class CreateSampleCommandValidator : AbstractValidator<CreateSampleCommand>
    {
        public CreateSampleCommandValidator()
        {
            RuleFor(x => x.Name).NotEmpty();
        }
    }

    private sealed record CountSampleQuery(int Value) : IQuery<int>;

    private sealed class CountSampleQueryHandler : IQueryHandler<CountSampleQuery, int>
    {
        public Task<int> HandleAsync(CountSampleQuery query, CancellationToken cancellationToken = default) =>
            Task.FromResult(query.Value * 2);
    }

    private sealed record MissingHandlerCommand() : ICommand;
}
