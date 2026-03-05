using Moq;
using NServiceBus;
using NUnit.Framework;
using SFA.DAS.Payments.Application.Infrastructure.Logging;
using SFA.DAS.Payments.EarningEvents.Messages.Events;
using SFA.DAS.Payments.FundingSource.Application.Interfaces;
using SFA.DAS.Payments.FundingSource.LevyFundedProxyService.Handlers;
using SFA.DAS.Payments.Model.Core;
using System;
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

        [SetUp]
        public void SetUp()
        {
            _loggerMock = new Mock<IPaymentLogger>();
            _serviceMock = new Mock<IReceivedDasEarningsService>();
            _contextMock = new Mock<IMessageHandlerContext>();
            _contextMock.SetupGet(c => c.MessageId).Returns("message-1");

            _handler = new ReceivedDASEarningsHandler(_loggerMock.Object, _serviceMock.Object);
        }

        [Test]
        public async Task Handle_CallsServiceAndLogsStartAndFinish()
        {
            var message = new DasEarningsReceivedEvent
            {
                EarningsId = Guid.NewGuid(),
                CourseCode = "COURSE1",
                CollectionPeriod = new CollectionPeriod { Period = 3 },
                UKPRN = 12345678,
                ULN = 87654321,
                LearningAimReference = "LAR"
            };

            await _handler.Handle(message, _contextMock.Object).ConfigureAwait(false);

            _serviceMock.Verify(s => s.RemovePreviousEarningsInCurrentCollection(It.Is<DasEarningsReceivedEvent>(m => m == message)), Times.Once);
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
            var message = new DasEarningsReceivedEvent
            {
                EarningsId = Guid.NewGuid(),
                CourseCode = "COURSE2",
                CollectionPeriod = new CollectionPeriod { Period = 1 },
                UKPRN = 11111111,
                ULN = 22222222,
                LearningAimReference = "REF"
            };

            var ex = new InvalidOperationException("boom");
            _serviceMock.Setup(s => s.RemovePreviousEarningsInCurrentCollection(It.IsAny<DasEarningsReceivedEvent>()))
                        .ThrowsAsync(ex);

            var act = new AsyncTestDelegate(() => _handler.Handle(message, _contextMock.Object));

            var thrown = Assert.ThrowsAsync<InvalidOperationException>(act);
            Assert.That(thrown, Is.SameAs(ex));
        }
    }
}