namespace mkl.eventbus
{
    public interface IEventHandler<TEvent>
        where TEvent : EventBase
    {
        void Handle(TEvent @event);
    }
}
