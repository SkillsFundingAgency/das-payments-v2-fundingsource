using Microsoft.ServiceFabric.Actors;
using Microsoft.ServiceFabric.Actors.Client;
using Moq;
using NServiceBus;
using NUnit.Framework;
using SFA.DAS.Payments.Application.Infrastructure.Logging;
using SFA.DAS.Payments.EarningEvents.Messages.Events;
using SFA.DAS.Payments.FundingSource.Application.Interfaces;
using SFA.DAS.Payments.FundingSource.LevyFundedProxyService.Handlers;
using SFA.DAS.Payments.Model.Core;
using System;
using System.Threading;
using System.Threading.Tasks;

namespace SFA.DAS.Payments.FundingSource.LevyFundedProxyService.UnitTests.Handlers
{
    [TestFixture]
    public class ReceivedDASEarningsHandlerTests
    {
        private Mock<IPaymentLogger> _loggerMock = null!;
        private Mock<IReceivedDasEarningsService> _serviceMock = null!;
        private Mock<IMessageHandlerContext> _contextMock = null!;
        private ReceivedDASEarningsHandler _handler = null!;

        private Mock<IActorProxyFactory> _proxyFactoryMock = null!;
        private readonly Mock<IReceivedDasEarningsService> _actorMock = new();


        [SetUp]
        public void SetUp()
        {
            _loggerMock = new Mock<IPaymentLogger>();
            _serviceMock = new Mock<IReceivedDasEarningsService>();
            _contextMock = new Mock<IMessageHandlerContext>();
            _contextMock.SetupGet(c => c.MessageId).Returns("message-1");
            _proxyFactoryMock = new Mock<IActorProxyFactory>();

            _handler = new ReceivedDASEarningsHandler(_loggerMock.Object, _proxyFactoryMock.Object);
        }

        [Test]
        public async Task Handle_CallsServiceAndLogsStartAndFinish()
        {
            // Arrange
            var message = new DasEarningsReceivedEvent
            {
                EarningsId = Guid.NewGuid(),
                CourseCode = "COURSE1",
                CollectionPeriod = new CollectionPeriod { Period = 3, AcademicYear = 2425 },
                UKPRN = 12345678,
                ULN = 87654321,
                LearningAimReference = "LAR"
            };

            var contextMock = new Mock<IMessageHandlerContext>();
            contextMock.Setup(x => x.MessageId).Returns(Guid.NewGuid().ToString());

            var loggerMock = new Mock<IPaymentLogger>();
            var proxyFactoryMock = new Mock<IActorProxyFactory>();
            var actorMock = new Mock<IReceivedDasEarningsService>();

            proxyFactoryMock
                .Setup(p => p.CreateActorProxy<IReceivedDasEarningsService>(
                    It.IsAny<Uri>(),
                    It.IsAny<ActorId>(),
                    It.IsAny<string>()))
                .Returns(actorMock.Object);

            var handler = new ReceivedDASEarningsHandler(
                loggerMock.Object, 
                proxyFactoryMock.Object);

            // Act
            await handler.Handle(message, contextMock.Object);

            // Assert
            actorMock.Verify(s =>
                s.RemovePreviousEarningsInCurrentCollection(
                    It.Is<DasEarningsReceivedEvent>(m => m == message),
                    It.IsAny<CancellationToken>()),
                Times.Once);
        }


        [Test]
        public void Handle_NullMessage_ThrowsArgumentNullException()
        {
            Assert.ThrowsAsync<ArgumentNullException>(async () => await _handler.Handle(null!, _contextMock.Object).ConfigureAwait(false));
        }

        [Test]
        public void Handle_NullContext_ThrowsArgumentNullException()
        {
            var message = new DasEarningsReceivedEvent { EarningsId = Guid.NewGuid() };
            Assert.ThrowsAsync<ArgumentNullException>(async () => await _handler.Handle(message, null!).ConfigureAwait(false));
        }

        [Test]
        public void Handle_WhenServiceThrows_LogsErrorAndRethrows()
        {
            // Arrange
            var message = new DasEarningsReceivedEvent
            {
                EarningsId = Guid.NewGuid(),
                CourseCode = "COURSE2",
                CollectionPeriod = new CollectionPeriod { Period = 1, AcademicYear = 2425 },
                UKPRN = 11111111,
                ULN = 22222222,
                LearningAimReference = "REF"
            };

            var contextMock = new Mock<IMessageHandlerContext>();
            contextMock.Setup(x => x.MessageId).Returns(Guid.NewGuid().ToString());

            var loggerMock = new Mock<IPaymentLogger>();
            var proxyFactoryMock = new Mock<IActorProxyFactory>();
            var actorMock = new Mock<IReceivedDasEarningsService>();

            var ex = new InvalidOperationException("boom");

            actorMock
                .Setup(s => s.RemovePreviousEarningsInCurrentCollection(
                    It.IsAny<DasEarningsReceivedEvent>(),
                    It.IsAny<CancellationToken>()))
                .ThrowsAsync(ex);

            proxyFactoryMock
                .Setup(p => p.CreateActorProxy<IReceivedDasEarningsService>(
                    It.IsAny<Uri>(),
                    It.IsAny<ActorId>(),
                    It.IsAny<string>()))
                .Returns(actorMock.Object);

            var handler = new ReceivedDASEarningsHandler(
                loggerMock.Object,
                proxyFactoryMock.Object);

            var act = new AsyncTestDelegate(() => handler.Handle(message, contextMock.Object));

            // Act
            var thrown = Assert.ThrowsAsync<InvalidOperationException>(act);

            // Assert
            Assert.That(thrown, Is.SameAs(ex));
        }
    }
}