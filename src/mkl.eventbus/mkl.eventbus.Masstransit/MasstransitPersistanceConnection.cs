using System;
using MassTransit;
using MassTransit.RabbitMqTransport;
using mkl.eventbus.Abstractions;
using mkl.eventbus.Masstransit.Policies;

namespace mkl.eventbus.Masstransit
{
    public class MasstransitPersistanceConnection : IPersitanceConnection<IRabbitMqHost, IBusControl>
    {
        public IRabbitMqHost Configurator { get; private set; }
        public IBusControl BusControl { get; private set; }
        public bool IsConnect => BusControl != null;
        private readonly string _endPoint;
        private readonly string _userName;
        private readonly string _password;

        public MasstransitPersistanceConnection(string endPoint, string userName, string password)
        {
            _endPoint = endPoint;
            _userName = userName;
            _password = password;

            Connect(BusCreationRetrivalPolicy.Default);
        }

        public void Connect(IRetrivalPolicy retrivalPolicy)
        {
            //TODO retrival
            BusControl = Bus.Factory.CreateUsingRabbitMq(cfg =>
           {
               Configurator = cfg.Host(new Uri(_endPoint), h =>
              {
                  h.Username(_userName);
                  h.Password(_password);
              });
           });

            BusControl.Start();
        }
    }
}
