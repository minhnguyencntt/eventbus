using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using MassTransit;
using mkl.eventbus.Abstractions;
using mkl.eventbus.Publishers;

namespace mkl.eventbus.Masstransit
{
    public class MasstransitEventBus : IEventBus
    {
        private Dictionary<string, HostReceiveEndpointHandle> _registeredEndPoint = new Dictionary<string, HostReceiveEndpointHandle>();
        private readonly IServiceProvider _serviceProvider;
        private readonly MasstransitPersistanceConnection _persistanceConnection;
        public MasstransitEventBus(
            MasstransitPersistanceConnection persitanceConnection,
            IServiceProvider serviceProvider
            )
        {
            _persistanceConnection = persitanceConnection;
            _serviceProvider = serviceProvider;
        }

        public IEventBus AddSubcription<TEvent, TEventHandler>()
            where TEvent : EventBase
            where TEventHandler : IEventHandler<TEvent>
        {
            AddSubcriptionInternal<TEvent, TEventHandler>(new DefaultPublisher());
            return this;
        }

        public IEventBus AddSubcription<TPublisher, TEvent, TEventHandler>()
            where TEvent : EventBase
            where TEventHandler : IEventHandler<TEvent>
        {
            var publisher = (IPublisher)_serviceProvider.GetService(typeof(TPublisher));
            if (publisher == null)
            {
                throw new ArgumentException(nameof(TPublisher));
            }
            AddSubcriptionInternal<TEvent, TEventHandler>(publisher);
            return this;
        }

        private void AddSubcriptionInternal<TEvent, TEventHandler>(IPublisher publisher)
             where TEvent : EventBase
            where TEventHandler : IEventHandler<TEvent>
        {
            if (!_registeredEndPoint.ContainsKey(publisher.Name))
            {
                var handle = _persistanceConnection.Configurator.ConnectReceiveEndpoint(publisher.Name, configure =>
                {
                    configure.Handler<TEvent>(context =>
                    {
                        var handler = (IEventHandler<TEvent>)_serviceProvider.GetService(typeof(TEventHandler));
                        if (handler == null)
                        {
                            throw new ArgumentException(nameof(IEventHandler<TEvent>));
                        }
                        handler.Handle(context.Message);
                        return Task.CompletedTask;
                    });
                });
                _registeredEndPoint.Add(publisher.Name, handle);
            }
            else
            {
                var handle = _registeredEndPoint[publisher.Name];
                handle.ReceiveEndpoint.ConnectHandler<TEvent>(context =>
                {
                    var handler = (IEventHandler<TEvent>)_serviceProvider.GetService(typeof(TEventHandler));
                    if (handler == null)
                    {
                        throw new ArgumentException(nameof(IEventHandler<TEvent>));
                    }
                    handler.Handle(context.Message);
                    return Task.CompletedTask;
                });
            }
        }

        public Task PublishAsync<TEvent>(TEvent @event)
            where TEvent : EventBase
        {
            var ev = _serviceProvider.GetService(typeof(TEvent));
            if (ev == null)
            {
                throw new ArgumentException(nameof(TEvent));
            }
            return _persistanceConnection.BusControl.Publish(ev);
        }
    }
}
