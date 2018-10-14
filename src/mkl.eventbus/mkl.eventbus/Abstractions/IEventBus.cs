using System.Threading.Tasks;

namespace mkl.eventbus.Abstractions
{
    public interface IEventBus
    {
        IEventBus AddSubcription<TEvent, TEventHandler>()
            where TEvent : EventBase
            where TEventHandler : IEventHandler<TEvent>;

        IEventBus AddSubcription<TPublisher, TEvent, TEventHandler>()
            where TEvent : EventBase
            where TEventHandler : IEventHandler<TEvent>;

        Task PublishAsync<TEvent>(TEvent @event)
            where TEvent : EventBase;
    }
}
