using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using FluentAssertions;
using Moq;
using NUnit.Framework;
using SFA.DAS.Payments.Application.Infrastructure.Logging;
using SFA.DAS.Payments.FundingSource.Application.Infrastructure.Configuration;
using SFA.DAS.Payments.FundingSource.Application.Repositories;
using SFA.DAS.Payments.FundingSource.Application.Services;
using SFA.DAS.Payments.FundingSource.Messages.Commands;
using SFA.DAS.Payments.Model.Core;
using SFA.DAS.Payments.Model.Core.Entities;
using SFA.DAS.Payments.Model.Core.Factories;
using SFA.DAS.Payments.Model.Core.OnProgramme;

namespace SFA.DAS.Payments.FundingSource.Application.UnitTests.Service
{
    [TestFixture]
    public class ApprenticeshipUnitCalculateOnProgrammePaymentStorageTests
    {
        private Mock<ILevyTransactionRepository> repository;
        private LevyTransactionBatchStorageService service;
        private List<LevyTransactionModel> savedModels;

        [SetUp]
        public void SetUp()
        {
            savedModels = new List<LevyTransactionModel>();
            repository = new Mock<ILevyTransactionRepository>();
            repository.Setup(x => x.SaveLevyTransactions(It.IsAny<IList<LevyTransactionModel>>(), It.IsAny<CancellationToken>()))
                .Callback<IList<LevyTransactionModel>, CancellationToken>((models, _) => savedModels.AddRange(models))
                .Returns(Task.CompletedTask);

            var autoMapper = AutoMapperConfigurationFactory.CreateMappingConfig().CreateMapper();
            service = new LevyTransactionBatchStorageService(Mock.Of<IPaymentLogger>(), repository.Object, autoMapper);
        }

        [TestCase(300)]
        [TestCase(-300)]
        public async Task Stored_Levy_Transaction_Has_Course_Type_Short_Course(int amount)
        {
            await service.StoreLevyTransactions(new List<CalculateOnProgrammePayment> { CreateCommand(amount) }, CancellationToken.None);

            savedModels.Single().CourseType.Should().Be(CourseType.ShortCourse);
        }

        [TestCase(300)]
        [TestCase(-300)]
        public async Task Stored_Levy_Transaction_Keeps_Learning_Type_And_Course_Code(int amount)
        {
            await service.StoreLevyTransactions(new List<CalculateOnProgrammePayment> { CreateCommand(amount) }, CancellationToken.None);

            savedModels.Single().LearningType.Should().Be(LearningType.ApprenticeshipUnit);
            savedModels.Single().CourseCode.Should().Be("ZSC0001");
            savedModels.Single().Amount.Should().Be(amount);
        }

        private static CalculateOnProgrammePayment CreateCommand(decimal amount)
        {
            return new CalculateOnProgrammePayment
            {
                AmountDue = amount,
                CollectionPeriod = CollectionPeriodFactory.CreateFromAcademicYearAndPeriod(2627, 2),
                DeliveryPeriod = 1,
                EventTime = DateTime.UtcNow,
                EventId = Guid.NewGuid(),
                Learner = new Learner { ReferenceNumber = "LEARNER1", Uln = 1000000001 },
                OnProgrammeEarningType = OnProgrammeEarningType.Milestone1,
                LearningAim = new LearningAim
                {
                    Reference = "ZSC0001",
                    CourseCode = "ZSC0001",
                    LearningType = LearningType.ApprenticeshipUnit,
                    FundingLineType = "GSO Short Courses (Apprenticeship Units) Levy"
                },
                PriceEpisodeIdentifier = "2627-P01",
                SfaContributionPercentage = 0.95m,
                Ukprn = 10000001,
                AccountId = 1000000,
                ApprenticeshipEmployerType = ApprenticeshipEmployerType.Levy,
                FundingPlatformType = FundingPlatformType.DigitalApprenticeshipService,
                StartDate = DateTime.Today.AddMonths(-1),
                PlannedEndDate = DateTime.Today.AddMonths(1)
            };
        }
    }
}
