using Microsoft.Extensions.DependencyInjection;

namespace SharedKernel.Abstraction;

public interface IDomainEvent : IEvent { }

public interface IEvent { }

public interface IEventHandler<TEvent> where TEvent : IEvent
{
    Task HandleAsync(TEvent @event, CancellationToken cancellationToken = default);
}

public interface IMediator
{
    Task PublishAsync<TEvent>(TEvent @event, CancellationToken cancellation = default) where TEvent : IEvent;
}

public class Mediator : IMediator
{
    private readonly IServiceProvider _serviceProvider;

    public Mediator(IServiceProvider serviceProvider)
    {
        _serviceProvider = serviceProvider;
    }

    public async Task PublishAsync<TEvent>(TEvent @event, CancellationToken cancellationToken = default) where TEvent : IEvent
    {
        var handlers = _serviceProvider.GetServices<IEventHandler<TEvent>>();

        var tasks = handlers.Select(handler => handler.HandleAsync(@event, cancellationToken));

        await Task.WhenAll(tasks);
    }
}