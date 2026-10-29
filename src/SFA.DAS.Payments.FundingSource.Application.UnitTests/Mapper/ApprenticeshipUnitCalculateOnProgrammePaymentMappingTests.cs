using AutoMapper;
using FluentAssertions;
using NUnit.Framework;
using SFA.DAS.Payments.FundingSource.Application.Infrastructure.Configuration;
using SFA.DAS.Payments.FundingSource.Messages.Commands;
using SFA.DAS.Payments.Model.Core;
using SFA.DAS.Payments.Model.Core.Entities;
using SFA.DAS.Payments.Model.Core.Factories;
using SFA.DAS.Payments.Model.Core.OnProgramme;
using SFA.DAS.Payments.RequiredPayments.Messages.Events;
using System;

namespace SFA.DAS.Payments.FundingSource.Application.UnitTests.Mapper
{
    [TestFixture]
    public class ApprenticeshipUnitCalculateOnProgrammePaymentMappingTests
    {
        private IMapper autoMapper;

        [SetUp]
        public void SetUp()
        {
            autoMapper = AutoMapperConfigurationFactory.CreateMappingConfig().CreateMapper();
        }

        [TestCase(300)]
        [TestCase(-300)]
        public void Apprenticeship_Unit_Command_Maps_Course_Type_Short_Course(int amount)
        {
            var command = CreateCommand(LearningType.ApprenticeshipUnit, amount);

            var requiredPayment = autoMapper.Map<CalculatedRequiredLevyAmount>(command);

            requiredPayment.CourseType.Should().Be(CourseType.ShortCourse);
        }

        [TestCase(LearningType.Apprenticeship)]
        [TestCase(LearningType.FoundationApprenticeship)]
        [TestCase(LearningType.MathsAndEnglish)]
        public void Other_Learning_Types_Keep_Course_Type_Apprenticeship(LearningType learningType)
        {
            var command = CreateCommand(learningType, 300);

            var requiredPayment = autoMapper.Map<CalculatedRequiredLevyAmount>(command);

            requiredPayment.CourseType.Should().Be(CourseType.Apprenticeship);
        }

        [Test]
        public void Apprenticeship_Unit_Command_Keeps_Learning_Aim_Details()
        {
            var command = CreateCommand(LearningType.ApprenticeshipUnit, -300);

            var requiredPayment = autoMapper.Map<CalculatedRequiredLevyAmount>(command);

            requiredPayment.LearningAim.LearningType.Should().Be(LearningType.ApprenticeshipUnit);
            requiredPayment.LearningAim.CourseCode.Should().Be("ZSC0001");
            requiredPayment.LearningAim.FundingLineType.Should().Be("GSO Short Courses (Apprenticeship Units) Levy");
            requiredPayment.AmountDue.Should().Be(-300);
        }

        private static CalculateOnProgrammePayment CreateCommand(LearningType learningType, decimal amount)
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
                    LearningType = learningType,
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
