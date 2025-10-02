using SharedKernel.Abstraction;

namespace SharedKernel.Mediator;

public interface IMediator
{
    Task PublishAsync<TEvent>(TEvent @event, CancellationToken cancellation = default) where TEvent : IEvent;
}