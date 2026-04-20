using Moq;
using NUnit.Framework;
using SFA.DAS.Payments.Application.Infrastructure.Logging;
using SFA.DAS.Payments.EarningEvents.Messages.Events;
using SFA.DAS.Payments.FundingSource.Application.Repositories;
using SFA.DAS.Payments.FundingSource.Application.Services;
using SFA.DAS.Payments.Model.Core;
using SFA.DAS.Payments.Model.Core.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace SFA.DAS.Payments.FundingSource.Application.UnitTests.Service
{
    [TestFixture]
    public class ReceivedDasEarningsProcessorTests
    {
        private Mock<ILevyTransactionRepository> _repositoryMock = null!;
        private Mock<IPaymentLogger> _loggerMock = null!;
        private ReceivedDasEarningsProcessor _processor = null!;
        private DasEarningsReceivedEvent _event;

        [SetUp]
        public void SetUp()
        {
            _repositoryMock = new Mock<ILevyTransactionRepository>();
            _loggerMock = new Mock<IPaymentLogger>();

            _event = new DasEarningsReceivedEvent
            {
                CollectionPeriod = new CollectionPeriod
                {
                    AcademicYear = 2526,
                    Period = 2
                },
                UKPRN = 10001234,
                CourseCode = "GSC1000",
                EarningsId = Guid.NewGuid(),
                LearningAimReference = "AIM123",
                ULN = 12345678
            };

            _processor = new ReceivedDasEarningsProcessor(_repositoryMock.Object, _loggerMock.Object);
        }

        [Test]
        public async Task RemovePreviousEarningsInCurrentCollection_WhenNoLevyTransactions_DoNotDeleteAndLogInfo()
        {
            // Arrange

            _repositoryMock
                .Setup(r => r.GetLevyTransactions(_event.CourseCode, _event.CollectionPeriod.AcademicYear,
                    _event.CollectionPeriod.Period, _event.UKPRN, _event.ULN, _event.LearningAimReference))
                .ReturnsAsync(new List<LevyTransactionModel>());

            // Act
            await _processor.RemovePreviousEarningsInCurrentCollection(_event, CancellationToken.None);

            // Assert
            _repositoryMock.Verify(
                r => r.GetLevyTransactions(_event.CourseCode, _event.CollectionPeriod.AcademicYear,
                    _event.CollectionPeriod.Period, _event.UKPRN, _event.ULN, _event.LearningAimReference), Times.Once);
            _repositoryMock.Verify(
                r => r.DeleteLevyTransactions(It.IsAny<IEnumerable<LevyTransactionModel>>(), It.IsAny<CancellationToken>()),
                Times.Never);
        }

        [Test]
        public async Task RemovePreviousEarningsInCurrentCollection_When_EventIsNewer_DeletesLevyTransactionAndLogs()
        {
            // Arrange
            var existingEarningId = new Guid("018f4d5e-9c7a-7a2d-b3f4-5c1a9e1b6a11");
            var messageEarningId = new Guid("018f4d5e-9c7a-7a2e-9a21-7d4c92c8b201");

            _event.EarningsId = messageEarningId;

            var levyModels = new List<LevyTransactionModel> 
            {
                new LevyTransactionModel
                {
                    EarningEventId = existingEarningId
                }
            };

            _repositoryMock
                .Setup(r => r.GetLevyTransactions(_event.CourseCode, _event.CollectionPeriod.AcademicYear,
                    _event.CollectionPeriod.Period, _event.UKPRN, _event.ULN, _event.LearningAimReference))
                .ReturnsAsync(levyModels);

            _repositoryMock
                .Setup(r => r.DeleteLevyTransactions(levyModels, It.IsAny<CancellationToken>()))
                .Returns(Task.CompletedTask)
                .Verifiable();

            // Act
            await _processor.RemovePreviousEarningsInCurrentCollection(_event, CancellationToken.None);

            // Assert
            _repositoryMock.Verify(r => r.DeleteLevyTransactions(levyModels, It.IsAny<CancellationToken>()), Times.Once);
        }

        [Test]
        public async Task RemovePreviousEarningsInCurrentCollection_WhenExistingIsNewer_DoesNotDeleteAndLogs()
        {
            // Arrange
            var existingEarningId = new Guid("019ce18d-7edb-701b-985c-545f8192eff2");
            var messageEarningId = new Guid("018f4d5e-9c7a-7a2e-9a21-7d4c92c8b201");

            _event.EarningsId = messageEarningId;

            var levyModels = new List<LevyTransactionModel>
            {
                new LevyTransactionModel
                {
                    EarningEventId = existingEarningId
                }
            };

            _repositoryMock
                .Setup(r => r.GetLevyTransactions(_event.CourseCode, _event.CollectionPeriod.AcademicYear,
                    _event.CollectionPeriod.Period, _event.UKPRN, _event.ULN, _event.LearningAimReference))
                .ReturnsAsync(levyModels);

            // Act
            await _processor.RemovePreviousEarningsInCurrentCollection(_event, CancellationToken.None);

            // Assert
            _repositoryMock.Verify(
                r => r.DeleteLevyTransactions(It.IsAny<IEnumerable<LevyTransactionModel>>(), It.IsAny<CancellationToken>()),
                Times.Never);
        }

        [Test]
        public void RemovePreviousEarningsInCurrentCollection_WhenRepositoryThrows_LogsErrorAndRethrows()
        {
            // Arrange
            var ex = new InvalidOperationException("repo failure");

            _repositoryMock
                .Setup(r => r.GetLevyTransactions(_event.CourseCode, _event.CollectionPeriod.AcademicYear,
                    _event.CollectionPeriod.Period, _event.UKPRN, _event.ULN, _event.LearningAimReference))
                .ThrowsAsync(ex);

            // Act & Assert
            var thrown = Assert.ThrowsAsync<InvalidOperationException>(async () =>
                await _processor.RemovePreviousEarningsInCurrentCollection(_event, CancellationToken.None));
            Assert.That(thrown, Is.SameAs(ex));
        }

        [Test]
        public async Task RemovePreviousEarningsInCurrentCollection_When_EventsAreNewer_DeletesLevyTransactions()
        {
            // Arrange
            var existingEarningId = new Guid("018f4d5e-9c7a-7a2d-b3f4-5c1a9e1b6a11");
            var messageEarningId = new Guid("018f4d5e-9c7a-7a2e-9a21-7d4c92c8b201");

            _event.EarningsId = messageEarningId;

            var levyModels = new List<LevyTransactionModel>
            {
                new LevyTransactionModel
                {
                    TransactionType = TransactionType.Milestone1,
                    EarningEventId = existingEarningId
                },
                new LevyTransactionModel
                {
                    TransactionType = TransactionType.Completion,
                    EarningEventId = existingEarningId
                }
            };

            _repositoryMock
                .Setup(r => r.GetLevyTransactions(_event.CourseCode, _event.CollectionPeriod.AcademicYear,
                    _event.CollectionPeriod.Period, _event.UKPRN, _event.ULN, _event.LearningAimReference))
                .ReturnsAsync(levyModels);

            _repositoryMock
                .Setup(r => r.DeleteLevyTransactions(levyModels, It.IsAny<CancellationToken>()))
                .Returns(Task.CompletedTask)
                .Verifiable();

            // Act
            await _processor.RemovePreviousEarningsInCurrentCollection(_event, CancellationToken.None);

            // Assert
            _repositoryMock.Verify(r => r.DeleteLevyTransactions(It.Is<IEnumerable<LevyTransactionModel>>(x => x.Count(x => x.EarningEventId == existingEarningId) == 2), It.IsAny<CancellationToken>()), Times.Once);
        }

        [Test]
        public void RemovePreviousEarningsInCurrentCollection_RejectsEmptyMessage()
        {
            // Arrange
            _event = null;
            var expectedException = new ArgumentNullException("message");

            // Act & Assert
            var thrown = Assert.ThrowsAsync<ArgumentNullException>(
                async () => await _processor.RemovePreviousEarningsInCurrentCollection(_event, CancellationToken.None)
                , expectedException.Message);
        }

        [Test]
        public void RemovePreviousEarningsInCurrentCollection_RejectsEmptyEarningsId()
        {
            // Arrange
            _event.EarningsId = Guid.Empty;
            var expectedException = new ArgumentException("EarningsId must be provided");

            // Act & Assert
            var thrown = Assert.ThrowsAsync<ArgumentException>(
                async () => await _processor.RemovePreviousEarningsInCurrentCollection(_event, CancellationToken.None)
                , expectedException.Message);
        }

        [Test]
        public void RemovePreviousEarningsInCurrentCollection_RejectsEmptyUKPRN()
        {
            // Arrange
            _event.UKPRN = 0;
            var expectedException = new ArgumentException("UKPRN must be provided");

            // Act & Assert
            var thrown = Assert.ThrowsAsync<ArgumentException>(
                async () => await _processor.RemovePreviousEarningsInCurrentCollection(_event, CancellationToken.None)
                , expectedException.Message);
        }

        [Test]
        public void RemovePreviousEarningsInCurrentCollection_RejectsEmptyULN()
        {
            // Arrange
            _event.ULN = 0;
            var expectedException = new ArgumentException("ULN must be provided");

            // Act & Assert
            var thrown = Assert.ThrowsAsync<ArgumentException>(
                async () => await _processor.RemovePreviousEarningsInCurrentCollection(_event, CancellationToken.None)
                , expectedException.Message);
        }

        [TestCase("")]
        [TestCase(null)]
        public void RemovePreviousEarningsInCurrentCollection_RejectsEmptyCourseCode(string courseCode)
        {
            // Arrange
            _event.CourseCode = courseCode;
            var expectedException = new ArgumentException("CourseCode must be provided");

            // Act & Assert
            var thrown = Assert.ThrowsAsync<ArgumentException>(
                async () => await _processor.RemovePreviousEarningsInCurrentCollection(_event, CancellationToken.None)
                , expectedException.Message);
        }

        [Test]
        public void RemovePreviousEarningsInCurrentCollection_RejectsEmptyCollectionPeriod()
        {
            // Arrange
            _event.CollectionPeriod = null;
            var expectedException = new ArgumentException("CollectionPeriod must be provided");

            // Act & Assert
            var thrown = Assert.ThrowsAsync<ArgumentException>(
                async () => await _processor.RemovePreviousEarningsInCurrentCollection(_event, CancellationToken.None)
                , expectedException.Message);
        }

        [Test]
        public void RemovePreviousEarningsInCurrentCollection_RejectsEmptyAcademicYear()
        {
            // Arrange
            _event.CollectionPeriod = new CollectionPeriod { AcademicYear = 0, Period = 1 };
            var expectedException = new ArgumentException("CollectionPeriod AcademicYear must be provided");

            // Act & Assert
            var thrown = Assert.ThrowsAsync<ArgumentException>(
                async () => await _processor.RemovePreviousEarningsInCurrentCollection(_event, CancellationToken.None)
                , expectedException.Message);
        }

        [Test]
        public void RemovePreviousEarningsInCurrentCollection_RejectsEmptyPeriod()
        {
            // Arrange
            _event.CollectionPeriod = new CollectionPeriod { AcademicYear = 2526, Period = 0 };
            var expectedException = new ArgumentException("CollectionPeriod Period must be provided");

            // Act & Assert
            var thrown = Assert.ThrowsAsync<ArgumentException>(
                async () => await _processor.RemovePreviousEarningsInCurrentCollection(_event, CancellationToken.None)
                , expectedException.Message);
        }

        [Test]
        public void RemovePreviousEarningsInCurrentCollection_RejectsInvalidPeriod()
        {
            // Arrange
            _event.CollectionPeriod = new CollectionPeriod { AcademicYear = 2526, Period = 15 };
            var expectedException = new ArgumentException("CollectionPeriod Period is invalid");

            // Act & Assert
            var thrown = Assert.ThrowsAsync<ArgumentException>(
                async () => await _processor.RemovePreviousEarningsInCurrentCollection(_event, CancellationToken.None)
                , expectedException.Message);
        }

        [TestCase("")]
        [TestCase(null)]
        public void RemovePreviousEarningsInCurrentCollection_RejectsEmptyLearningAimReference(string learningAimReference)
        {
            // Arrange
            _event.LearningAimReference = learningAimReference;
            var expectedException = new ArgumentException("LearningAimReference must be provided");

            // Act & Assert
            var thrown = Assert.ThrowsAsync<ArgumentException>(
                async () => await _processor.RemovePreviousEarningsInCurrentCollection(_event, CancellationToken.None)
                , expectedException.Message);
        }
    }
}

