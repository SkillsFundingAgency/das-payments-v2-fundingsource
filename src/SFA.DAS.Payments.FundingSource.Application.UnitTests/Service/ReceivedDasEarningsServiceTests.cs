using FluentAssertions.Common;
using Moq;
using NUnit.Framework;
using SFA.DAS.Payments.Application.Infrastructure.Logging;
using SFA.DAS.Payments.EarningEvents.Messages.Events;
using SFA.DAS.Payments.FundingSource.Application.Repositories;
using SFA.DAS.Payments.FundingSource.Application.Services;
using SFA.DAS.Payments.Model.Core;
using SFA.DAS.Payments.Model.Core.Entities;
using System;
using System.Threading;
using System.Threading.Tasks;

namespace SFA.DAS.Payments.FundingSource.Application.UnitTests.Service
{
    [TestFixture]
    public class ReceivedDasEarningsServiceTests
    {
        private Mock<ILevyTransactionRepository> _repositoryMock = null!;
        private Mock<IPaymentLogger> _loggerMock = null!;
        private ReceivedDasEarningsService _service = null!;

        [SetUp]
        public void SetUp()
        {
            _repositoryMock = new Mock<ILevyTransactionRepository>();
            _loggerMock = new Mock<IPaymentLogger>();

            _service = new ReceivedDasEarningsService(_repositoryMock.Object, _loggerMock.Object);
        }

        [Test]
        public async Task RemovePreviousEarningsInCurrentCollection_WhenNoLevyTransactions_DoNotDeleteAndLogInfo()
        {
            // Arrange
            var message = new DasEarningsReceivedEvent
            {
                EarningsId = Guid.NewGuid(),
                CourseCode = "C1",
                CollectionPeriod = new CollectionPeriod { Period = 5, AcademicYear = 2425 },
                UKPRN = 1000L,
                ULN = 2000L,
                LearningAimReference = "LAR1"
            };

            _repositoryMock
                .Setup(r => r.GetLevyTransactionAsync(message.CourseCode, message.CollectionPeriod.AcademicYear, message.CollectionPeriod.Period, message.UKPRN, message.ULN, message.LearningAimReference))
                .ReturnsAsync((LevyTransactionModel)null);

            // Act
            await _service.RemovePreviousEarningsInCurrentCollection(message, CancellationToken.None);

            // Assert
            _repositoryMock.Verify(r => r.GetLevyTransactionAsync(message.CourseCode, message.CollectionPeriod.AcademicYear,message.CollectionPeriod.Period, message.UKPRN, message.ULN, message.LearningAimReference), Times.Once);
            _repositoryMock.Verify(r => r.DeleteLevyTransaction(It.IsAny<LevyTransactionModel>(), It.IsAny<CancellationToken>()), Times.Never);
        }

        [Test]
        public async Task RemovePreviousEarningsInCurrentCollection_WhenMessageIsNewer_DeletesLevyTransactionAndLogs()
        {
            // Arrange
            var existingEarningId = Guid.NewGuid();
            var messageEarningId = Guid.NewGuid();
            existingEarningId = new Guid("018f4d5e-9c7a-7a2d-b3f4-5c1a9e1b6a11");
            messageEarningId = new Guid("018f4d5e-9c7a-7a2e-9a21-7d4c92c8b201");

            var message = new DasEarningsReceivedEvent
            {
                EarningsId = messageEarningId,
                CourseCode = "C2",
                CollectionPeriod = new CollectionPeriod { Period = 2 , AcademicYear = 2425 },
                UKPRN = 1111L,
                ULN = 2222L,
                LearningAimReference = "LAR2"
            };

            var levyModel = new LevyTransactionModel
            {
                EarningEventId = existingEarningId
            };

            _repositoryMock
                .Setup(r => r.GetLevyTransactionAsync(message.CourseCode, message.CollectionPeriod.AcademicYear,message.CollectionPeriod.Period, message.UKPRN, message.ULN, message.LearningAimReference))
                .ReturnsAsync(levyModel);

            _repositoryMock
                .Setup(r => r.DeleteLevyTransaction(levyModel, It.IsAny<CancellationToken>()))
                .Returns(Task.CompletedTask)
                .Verifiable();

            // Act
            await _service.RemovePreviousEarningsInCurrentCollection(message, CancellationToken.None);

            // Assert
            _repositoryMock.Verify(r => r.DeleteLevyTransaction(levyModel, It.IsAny<CancellationToken>()), Times.Once);
        }

        [Test]
        public async Task RemovePreviousEarningsInCurrentCollection_WhenExistingIsNewer_DoesNotDeleteAndLogs()
        {
            // Arrange
            var existingEarningId = new Guid("019ce18d-7edb-701b-985c-545f8192eff2");
            var messageEarningId = new Guid("018f4d5e-9c7a-7a2e-9a21-7d4c92c8b201");

            var message = new DasEarningsReceivedEvent
            {
                EarningsId = messageEarningId,
                CourseCode = "C3",
                CollectionPeriod = new CollectionPeriod { Period = 1 , AcademicYear = 2425 },
                UKPRN = 2222L,
                ULN = 3333L,
                LearningAimReference = "LAR3"
            };

            var levyModel = new LevyTransactionModel
            {
                EarningEventId = existingEarningId
            };

            _repositoryMock
                .Setup(r => r.GetLevyTransactionAsync(message.CourseCode, message.CollectionPeriod.AcademicYear, message.CollectionPeriod.Period, message.UKPRN, message.ULN, message.LearningAimReference))
                .ReturnsAsync(levyModel);

            // Act
            await _service.RemovePreviousEarningsInCurrentCollection(message, CancellationToken.None);

            // Assert
            _repositoryMock.Verify(r => r.DeleteLevyTransaction(It.IsAny<LevyTransactionModel>(), It.IsAny<CancellationToken>()), Times.Never);
        }

        [Test]
        public void RemovePreviousEarningsInCurrentCollection_WhenRepositoryThrows_LogsErrorAndRethrows()
        {
            // Arrange
            var message = new DasEarningsReceivedEvent
            {
                EarningsId = Guid.NewGuid(),
                CourseCode = "C4",
                CollectionPeriod = new CollectionPeriod { Period = 4 , AcademicYear = 2425 },
                UKPRN = 3333L,
                ULN = 4444L,
                LearningAimReference = "LAR4"
            };

            var ex = new InvalidOperationException("repo failure");

            _repositoryMock
                .Setup(r => r.GetLevyTransactionAsync(message.CourseCode, message.CollectionPeriod.AcademicYear, message.CollectionPeriod.Period, message.UKPRN, message.ULN, message.LearningAimReference))
                .ThrowsAsync(ex);

            // Act & Assert
            var thrown = Assert.ThrowsAsync<InvalidOperationException>(async () => await _service.RemovePreviousEarningsInCurrentCollection(message, CancellationToken.None));
            Assert.That(thrown, Is.SameAs(ex));
        }
    }
}