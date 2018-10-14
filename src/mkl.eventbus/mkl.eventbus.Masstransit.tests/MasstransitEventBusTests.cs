using System;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using mkl.eventbus.Abstractions;
using mkl.eventbus.Masstransit.tests.Mocks;
using Moq;

namespace mkl.eventbus.Masstransit.tests
{
    [TestClass]
    public class MasstransitEventBusTests
    {
        private IEventBus _eventBus;
        private Mock<IServiceProvider> _mockServiceProvider;

        [TestInitialize]
        public void Setup()
        {
            _mockServiceProvider = new Mock<IServiceProvider>();
            _eventBus = new MasstransitEventBus(
                new MasstransitPersistanceConnection("rabbitmq://localhost", "guest", "guest"),
                _mockServiceProvider.Object);
        }

        [TestMethod]
        public async Task Can_Subcribe_Event_Default_Publisher()
        {
            var actualValue = 0;
            var domainEvent = new MockEvent()
            {
                Id = 100
            };

            var mockDomainEventHandler = new Mock<MockEventHandler>();

            mockDomainEventHandler
                .Setup(x => x.Handle(It.IsAny<MockEvent>()))
                .Callback(() =>
                {
                    actualValue = 101;
                });

            _mockServiceProvider
                .Setup(x => x.GetService(It.Is<Type>(type => type == typeof(MockEvent))))
                .Returns(domainEvent);

            _mockServiceProvider
                .Setup(x => x.GetService(It.Is<Type>(type => type == typeof(MockEventHandler))))
                .Returns(mockDomainEventHandler.Object);

            _eventBus.AddSubcription<MockEvent, MockEventHandler>();

            var mockEventBus = new Mock<IEventBus>();

            mockEventBus.Setup(x => x.PublishAsync(It.IsAny<MockEvent>()))
                .Returns(Task.Run(() =>
                {
                    _eventBus.PublishAsync(domainEvent);
                    Thread.Sleep(2000);
                }));

            await mockEventBus.Object.PublishAsync(domainEvent);
            Assert.AreEqual(101, actualValue);
        }

        [TestMethod]
        public async Task Can_Subcribes_Event_Default_Publisher()
        {
            var actualValue = 0;
            var anotherActualValue = 0;
            var domainEvent = new MockEvent()
            {
                Id = 100
            };

            var mockDomainEventHandler = new Mock<MockEventHandler>();

            mockDomainEventHandler
                .Setup(x => x.Handle(It.IsAny<MockEvent>()))
                .Callback(() =>
                {
                    actualValue = 101;
                });

            var mockDomainEventHandler2 = new Mock<MockEventHandler>();

            mockDomainEventHandler2
                .Setup(x => x.Handle(It.IsAny<MockEvent>()))
                .Callback(() =>
                {
                    anotherActualValue = 102;
                });

            _mockServiceProvider
                .Setup(x => x.GetService(It.Is<Type>(type => type == typeof(MockEvent))))
                .Returns(domainEvent);

            _mockServiceProvider
                .Setup(x => x.GetService(It.Is<Type>(type => type == typeof(MockEventHandler))))
                .Returns(mockDomainEventHandler.Object);

            _mockServiceProvider
                .Setup(x => x.GetService(It.Is<Type>(type => type == typeof(AnotherMockEventHandler))))
                .Returns(mockDomainEventHandler2.Object);

            _eventBus.AddSubcription<MockEvent, MockEventHandler>();
            _eventBus.AddSubcription<MockEvent, AnotherMockEventHandler>();

            var mockEventBus = new Mock<IEventBus>();

            mockEventBus.Setup(x => x.PublishAsync(It.IsAny<MockEvent>()))
                .Returns(Task.Run(() =>
                {
                    _eventBus.PublishAsync(domainEvent);
                    Thread.Sleep(2000);
                }));

            await mockEventBus.Object.PublishAsync(domainEvent);
            Assert.AreEqual(101, actualValue);
            Assert.AreEqual(102, anotherActualValue);
        }

        [TestMethod]
        public async Task Can_Subcribe_Event_Specific_Publisher()
        {
            var actualValue = 0;
            var domainEvent = new MockEvent()
            {
                Id = 100
            };

            var mockDomainEventHandler = new Mock<MockEventHandler>();

            mockDomainEventHandler
                .Setup(x => x.Handle(It.IsAny<MockEvent>()))
                .Callback(() =>
                {
                    actualValue = 101;
                });

            _mockServiceProvider
                .Setup(x => x.GetService(It.Is<Type>(type => type == typeof(MockEvent))))
                .Returns(domainEvent);

            _mockServiceProvider
                .Setup(x => x.GetService(It.Is<Type>(type => type == typeof(MockEventHandler))))
                .Returns(mockDomainEventHandler.Object);

            _mockServiceProvider
                .Setup(x => x.GetService(It.Is<Type>(type => type == typeof(MockPublisher))))
                .Returns(new MockPublisher());

            _eventBus.AddSubcription<MockPublisher, MockEvent, MockEventHandler>();

            var mockEventBus = new Mock<IEventBus>();

            mockEventBus.Setup(x => x.PublishAsync(It.IsAny<MockEvent>()))
                .Returns(Task.Run(() =>
                {
                    _eventBus.PublishAsync(domainEvent);
                    Thread.Sleep(2000);
                }));

            await mockEventBus.Object.PublishAsync(domainEvent);
            Assert.AreEqual(101, actualValue);
        }

        [TestMethod]
        public async Task Can_Subcribes_Event_Specific_Publisher()
        {
            var actualValue = 0;
            var anotherActualValue = 0;
            var domainEvent = new MockEvent()
            {
                Id = 100
            };

            var mockDomainEventHandler = new Mock<MockEventHandler>();

            mockDomainEventHandler
                .Setup(x => x.Handle(It.IsAny<MockEvent>()))
                .Callback(() =>
                {
                    actualValue = 101;
                });

            var mockDomainEventHandler2 = new Mock<MockEventHandler>();

            mockDomainEventHandler2
                .Setup(x => x.Handle(It.IsAny<MockEvent>()))
                .Callback(() =>
                {
                    anotherActualValue = 102;
                });

            _mockServiceProvider
                .Setup(x => x.GetService(It.Is<Type>(type => type == typeof(MockEvent))))
                .Returns(domainEvent);

            _mockServiceProvider
                .Setup(x => x.GetService(It.Is<Type>(type => type == typeof(MockEventHandler))))
                .Returns(mockDomainEventHandler.Object);

            _mockServiceProvider
                .Setup(x => x.GetService(It.Is<Type>(type => type == typeof(AnotherMockEventHandler))))
                .Returns(mockDomainEventHandler2.Object);

            _mockServiceProvider
                .Setup(x => x.GetService(It.Is<Type>(type => type == typeof(MockPublisher))))
                .Returns(new MockPublisher());


            _eventBus.AddSubcription<MockPublisher, MockEvent, MockEventHandler>();
            _eventBus.AddSubcription<MockPublisher, MockEvent, AnotherMockEventHandler>();

            var mockEventBus = new Mock<IEventBus>();

            mockEventBus.Setup(x => x.PublishAsync(It.IsAny<MockEvent>()))
                .Returns(Task.Run(() =>
                {
                    _eventBus.PublishAsync(domainEvent);
                    Thread.Sleep(2000);
                }));

            await mockEventBus.Object.PublishAsync(domainEvent);
            Assert.AreEqual(101, actualValue);
            Assert.AreEqual(102, anotherActualValue);
        }

        [TestMethod]
        public async Task Can_Subcribes_Event_On_Two_Publisher()
        {
            var actualValue = 0;
            var anotherActualValue = 0;
            var domainEvent = new MockEvent()
            {
                Id = 100
            };

            var mockDomainEventHandler = new Mock<MockEventHandler>();

            mockDomainEventHandler
                .Setup(x => x.Handle(It.IsAny<MockEvent>()))
                .Callback(() =>
                {
                    actualValue = 101;
                });

            var mockDomainEventHandler2 = new Mock<MockEventHandler>();

            mockDomainEventHandler2
                .Setup(x => x.Handle(It.IsAny<MockEvent>()))
                .Callback(() =>
                {
                    anotherActualValue = 102;
                });

            _mockServiceProvider
                .Setup(x => x.GetService(It.Is<Type>(type => type == typeof(MockEvent))))
                .Returns(domainEvent);

            _mockServiceProvider
                .Setup(x => x.GetService(It.Is<Type>(type => type == typeof(MockEventHandler))))
                .Returns(mockDomainEventHandler.Object);

            _mockServiceProvider
                .Setup(x => x.GetService(It.Is<Type>(type => type == typeof(AnotherMockEventHandler))))
                .Returns(mockDomainEventHandler2.Object);

            _mockServiceProvider
                .Setup(x => x.GetService(It.Is<Type>(type => type == typeof(MockPublisher))))
                .Returns(new MockPublisher());


            _eventBus.AddSubcription<MockEvent, MockEventHandler>();
            _eventBus.AddSubcription<MockPublisher, MockEvent, AnotherMockEventHandler>();

            var mockEventBus = new Mock<IEventBus>();

            mockEventBus.Setup(x => x.PublishAsync(It.IsAny<MockEvent>()))
                .Returns(Task.Run(() =>
                {
                    _eventBus.PublishAsync(domainEvent);
                    Thread.Sleep(2000);
                }));

            await mockEventBus.Object.PublishAsync(domainEvent);
            Assert.AreEqual(101, actualValue);
            Assert.AreEqual(102, anotherActualValue);
        }


        [TestCleanup]
        public void CleanUp()
        {

        }
    }
}
