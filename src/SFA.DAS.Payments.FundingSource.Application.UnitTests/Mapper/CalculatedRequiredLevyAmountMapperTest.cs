using AutoMapper;
using FluentAssertions;
using NUnit.Framework;
using SFA.DAS.Payments.FundingSource.Application.Infrastructure.Configuration;
using SFA.DAS.Payments.FundingSource.Messages.Events;
using SFA.DAS.Payments.Model.Core;
using SFA.DAS.Payments.Model.Core.Entities;
using SFA.DAS.Payments.Model.Core.Factories;
using SFA.DAS.Payments.Model.Core.OnProgramme;
using SFA.DAS.Payments.RequiredPayments.Messages.Events;
using System;
using SFA.DAS.Payments.FundingSource.Messages.Commands;

namespace SFA.DAS.Payments.FundingSource.Application.UnitTests.Mapper
{

    [TestFixture]
    public class CalculateOnProgrammePaymentToCalculatedRequiredLevyAmountMappingTests
    {
        private IMapper autoMapper;
        private MapperConfiguration mapperConfiguration;
        private CalculateOnProgrammePayment onProgrammePaymentCommand;

        [SetUp]
        public void Setup()
        {
            onProgrammePaymentCommand = new CalculateOnProgrammePayment
            {
                AmountDue = 1000.00m,
                CollectionPeriod = CollectionPeriodFactory.CreateFromAcademicYearAndPeriod(1819, 1),
                DeliveryPeriod = 1,
                EventTime = DateTime.UtcNow,
                Learner = new Learner
                {
                    ReferenceNumber = "001",
                    Uln = 1234567890
                },
                OnProgrammeEarningType = OnProgrammeEarningType.Learning,
                LearningAim = new LearningAim
                {
                    FrameworkCode = 403
                },
                PriceEpisodeIdentifier = "1819-P01",
                SfaContributionPercentage = 0.9m,
                Ukprn = 10000,
                EventId = Guid.NewGuid(),
                AccountId = 1000000,
                ApprenticeshipEmployerType = ApprenticeshipEmployerType.Levy,
                ApprenticeshipId = 12,
                TransferSenderAccountId = 1000000,
                FundingPlatformType = FundingPlatformType.DigitalApprenticeshipService,
                ActualEndDate = DateTime.Today,
                AgreedOnDate = DateTime.Today.AddYears(-1),
                ApprenticeshipPriceEpisodeId = 1,
                CompletionAmount = 3000, 
                CompletionStatus = 1,
                InstalmentAmount = 1000.0m,
                LearningStartDate = DateTime.Today.AddYears(-1),
                NumberOfInstalments = 12,
                PlannedEndDate = DateTime.Today,
                StartDate = DateTime.Today.AddYears(-1)
            };
            mapperConfiguration = AutoMapperConfigurationFactory.CreateMappingConfig();
            autoMapper = mapperConfiguration.CreateMapper();
        }

        [Test]
        public void Maps_Corresponding_Properties()
        {
            var destination =
                autoMapper.Map<CalculateOnProgrammePayment, CalculatedRequiredLevyAmount>(onProgrammePaymentCommand);
            destination.Should().BeEquivalentTo(onProgrammePaymentCommand);
        }

        [TestCase(OnProgrammeEarningType.Learning, TransactionType.Learning)]
        [TestCase(OnProgrammeEarningType.Completion, TransactionType.Completion)]
        [TestCase(OnProgrammeEarningType.Balancing, TransactionType.Balancing)]
        public void Maps_Transaction_Type_From_OnProgrammeEarningType(OnProgrammeEarningType sourceEarningType, TransactionType destinationEarningType)
        {
            onProgrammePaymentCommand.OnProgrammeEarningType = sourceEarningType;
            var destination =
                autoMapper.Map<CalculateOnProgrammePayment, CalculatedRequiredLevyAmount>(onProgrammePaymentCommand);
            destination.TransactionType.Should().Be(destinationEarningType);
        }

        [Test]
        public void Populates_Specified_Default_Values()
        {
            //defaults are documented here: https://skillsfundingagency.atlassian.net/wiki/spaces/NDL/pages/4468211759/Payments+BAU+-+Payments+Simplification
            var destination =
                autoMapper.Map<CalculateOnProgrammePayment, CalculatedRequiredLevyAmount>(onProgrammePaymentCommand);
            destination.Should().BeEquivalentTo(onProgrammePaymentCommand);
            destination.Priority.Should().Be(0);
            destination.AgreementId.Should().BeNullOrEmpty();
            destination.EarningEventId.Should().BeEmpty();
            destination.ClawbackSourcePaymentEventId.Should().BeEmpty();
            destination.ContractType.Should().Be(ContractType.Act1);
            destination.ReportingAimFundingLineType.Should().BeNullOrEmpty();
            destination.LearningAimSequenceNumber.Should().Be(0);
            destination.JobId.Should().Be(-1);
            destination.IlrSubmissionDateTime.Should().Be(new DateTime(1753,1,1));
            destination.IlrFileName.Should().BeNullOrEmpty();
        }
    }

    [TestFixture]
    public class CalculatedRequiredLevyAmountMapperTest
    {
        private IMapper autoMapper;
        private MapperConfiguration mapperConfiguration;
        private CalculatedRequiredLevyAmount requiredPaymentEvent;

        [SetUp]
        public void Setup()
        {
            requiredPaymentEvent = new CalculatedRequiredLevyAmount
            {
                AmountDue = 1000.00m,
                CollectionPeriod = CollectionPeriodFactory.CreateFromAcademicYearAndPeriod(1819, 1),
                DeliveryPeriod = 1,
                EventTime = DateTime.UtcNow,
                JobId = 1,
                Learner = new Learner
                {
                    ReferenceNumber = "001",
                    Uln = 1234567890
                },
                OnProgrammeEarningType = OnProgrammeEarningType.Learning,
                LearningAim = new LearningAim
                {
                    FrameworkCode = 403,
                    LearningType = LearningType.Apprenticeship,
                    CourseCode = "COURSE1"
                },
                PriceEpisodeIdentifier = "1819-P01",
                SfaContributionPercentage = 0.9m,
                Ukprn = 10000,

                AgreementId = "11",
                Priority = 13,
                EventId = Guid.NewGuid(),
                AccountId = 1000000,
                IlrSubmissionDateTime = DateTime.Today,
                EarningEventId = Guid.NewGuid(),
                ContractType = ContractType.Act1,
                ApprenticeshipEmployerType = ApprenticeshipEmployerType.Levy,
                ApprenticeshipId = 12,
                AgeAtStartOfLearning = 17,
                CourseType = CourseType.Apprenticeship,
                FundingPlatformType = FundingPlatformType.DigitalApprenticeshipService,
                ReportingAimFundingLineType = "funding line type"
            };
            mapperConfiguration = AutoMapperConfigurationFactory.CreateMappingConfig();
            autoMapper = mapperConfiguration.CreateMapper();
        }

        [TestCase(LearningType.Apprenticeship)]
        [TestCase(LearningType.ApprenticeshipUnit)]
        public void TestMapToLevyFundingSourcePaymentEvent_WithDifferentLearningTypes(LearningType learningType)
        {
            requiredPaymentEvent.LearningAim.LearningType = learningType;
            var expectedEvent = new LevyFundingSourcePaymentEvent
            {
                AgreementId = "11"
            };

            PopulateCommonProperties(expectedEvent);

            var actualEvent = new LevyFundingSourcePaymentEvent
            {
                AmountDue = 55,
                FundingSourceType = FundingSourceType.Levy
            };

            // act
            autoMapper.Map(requiredPaymentEvent, actualEvent);

            // assert
            actualEvent.EventTime.Should().NotBe(expectedEvent.EventTime);
            actualEvent.EventId.Should().NotBe(expectedEvent.EventId);
            actualEvent.EventTime = expectedEvent.EventTime;
            actualEvent.EventId = expectedEvent.EventId;
            actualEvent.Should().BeEquivalentTo(expectedEvent);
            actualEvent.AgeAtStartOfLearning.Should().Be(17);
            actualEvent.FundingPlatformType.Should().Be(expectedEvent.FundingPlatformType);
            actualEvent.LearningAim.CourseCode.Should().Be(expectedEvent.LearningAim.CourseCode);
            actualEvent.ReportingAimFundingLineType.Should().Be(expectedEvent.ReportingAimFundingLineType);
            actualEvent.LearningAim.LearningType.Should().Be(expectedEvent.LearningAim.LearningType);
            actualEvent.CourseType.Should().Be(expectedEvent.CourseType);
            actualEvent.LearningAim.LearningType.Should().Be(learningType);
        }

        [TestCase(LearningType.Apprenticeship)]
        [TestCase(LearningType.ApprenticeshipUnit)]
        public void TestMapToValidSfaCoInvestedFundingSourcePaymentEvent(LearningType learningType)
        {
            requiredPaymentEvent.LearningAim.LearningType = learningType;
            var expectedEvent = new SfaCoInvestedFundingSourcePaymentEvent();
            PopulateCommonProperties(expectedEvent);

            var actualEvent = new LevyFundingSourcePaymentEvent
            {
                AmountDue = 55,
                FundingSourceType = FundingSourceType.Levy
            };

            // act
            autoMapper.Map(requiredPaymentEvent, actualEvent);

            // assert
            actualEvent.EventTime.Should().NotBe(expectedEvent.EventTime);
            actualEvent.EventId.Should().NotBe(expectedEvent.EventId);
            actualEvent.EventTime = expectedEvent.EventTime;
            actualEvent.EventId = expectedEvent.EventId;
            actualEvent.Should().BeEquivalentTo(expectedEvent);
            actualEvent.AgeAtStartOfLearning.Should().Be(17);
            actualEvent.LearningAim.LearningType.Should().Be(learningType);
        }

        [TestCase(LearningType.Apprenticeship)]
        [TestCase(LearningType.ApprenticeshipUnit)]
        public void TestMapToValidEmployerCoInvestedFundingSourcePaymentEvent(LearningType learningType)
        {
            requiredPaymentEvent.LearningAim.LearningType = learningType;
            var expectedEvent = new EmployerCoInvestedFundingSourcePaymentEvent();
            PopulateCommonProperties(expectedEvent);

            var actualEvent = new LevyFundingSourcePaymentEvent
            {
                AmountDue = 55,
                FundingSourceType = FundingSourceType.Levy
            };

            // act
            autoMapper.Map(requiredPaymentEvent, actualEvent);

            // assert
            actualEvent.EventTime.Should().NotBe(expectedEvent.EventTime);
            actualEvent.EventId.Should().NotBe(expectedEvent.EventId);
            actualEvent.EventTime = expectedEvent.EventTime;
            actualEvent.EventId = expectedEvent.EventId;
            actualEvent.Should().BeEquivalentTo(expectedEvent);
            actualEvent.AgeAtStartOfLearning.Should().Be(17);
            actualEvent.LearningAim.LearningType.Should().Be(learningType);
        }

        [TestCase(LearningType.Apprenticeship)]
        [TestCase(LearningType.ApprenticeshipUnit)]
        public void TestMapToValidSfaFullyFundedFundingSourcePaymentEvent(LearningType learningType)
        {
            requiredPaymentEvent.LearningAim.LearningType = learningType;
            var expectedEvent = new SfaFullyFundedFundingSourcePaymentEvent();
            PopulateCommonProperties(expectedEvent);

            var actualEvent = new LevyFundingSourcePaymentEvent
            {
                AmountDue = 55,
                FundingSourceType = FundingSourceType.Levy
            };

            // act
            autoMapper.Map(requiredPaymentEvent, actualEvent);

            // assert
            actualEvent.EventTime.Should().NotBe(expectedEvent.EventTime);
            actualEvent.EventId.Should().NotBe(expectedEvent.EventId);
            actualEvent.EventTime = expectedEvent.EventTime;
            actualEvent.EventId = expectedEvent.EventId;
            actualEvent.Should().BeEquivalentTo(expectedEvent);
            actualEvent.AgeAtStartOfLearning.Should().Be(17);
            actualEvent.LearningAim.LearningType.Should().Be(learningType);
        }

        private void PopulateCommonProperties(FundingSourcePaymentEvent expectedEvent)
        {
            expectedEvent.FundingSourceType = FundingSourceType.Levy;
            expectedEvent.ApprenticeshipId = 12;
            expectedEvent.Learner = new Learner
            {
                ReferenceNumber = "001",
                Uln = 1234567890
            };
            expectedEvent.JobId = 1;
            expectedEvent.ContractType = ContractType.Act1;
            expectedEvent.Ukprn = 10000;
            expectedEvent.AmountDue = 55;
            expectedEvent.TransactionType = TransactionType.Learning;
            expectedEvent.LearningAim = requiredPaymentEvent.LearningAim;
            expectedEvent.DeliveryPeriod = 1;
            expectedEvent.IlrSubmissionDateTime = DateTime.Today;
            expectedEvent.RequiredPaymentEventId = requiredPaymentEvent.EventId;
            expectedEvent.PriceEpisodeIdentifier = "1819-P01";
            expectedEvent.SfaContributionPercentage = .9m;
            expectedEvent.CollectionPeriod = CollectionPeriodFactory.CreateFromAcademicYearAndPeriod(1819, 1);
            expectedEvent.AccountId = 1000000;
            expectedEvent.EarningEventId = requiredPaymentEvent.EarningEventId;
            expectedEvent.ApprenticeshipEmployerType = requiredPaymentEvent.ApprenticeshipEmployerType;
            expectedEvent.AgeAtStartOfLearning = requiredPaymentEvent.AgeAtStartOfLearning;
            expectedEvent.FundingPlatformType = requiredPaymentEvent.FundingPlatformType;
            expectedEvent.CourseType = requiredPaymentEvent.CourseType;
            expectedEvent.ReportingAimFundingLineType = requiredPaymentEvent.ReportingAimFundingLineType;
        }
    }
}