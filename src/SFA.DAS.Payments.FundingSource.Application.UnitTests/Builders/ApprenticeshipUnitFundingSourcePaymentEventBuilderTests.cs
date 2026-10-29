using System.Collections.Generic;
using System.Linq;
using FluentAssertions;
using Moq;
using NUnit.Framework;
using SFA.DAS.Payments.FundingSource.Application.Builders;
using SFA.DAS.Payments.FundingSource.Application.Infrastructure.Configuration;
using SFA.DAS.Payments.FundingSource.Application.UnitTests.Mapper;
using SFA.DAS.Payments.FundingSource.Domain.Interface;
using SFA.DAS.Payments.FundingSource.Domain.Models;
using SFA.DAS.Payments.FundingSource.Messages.Events;
using SFA.DAS.Payments.Model.Core.Entities;

namespace SFA.DAS.Payments.FundingSource.Application.UnitTests.Builders
{
    [TestFixture]
    public class ApprenticeshipUnitFundingSourcePaymentEventBuilderTests
    {
        [TestCase(FundingSourceType.Levy, 300)]
        [TestCase(FundingSourceType.Levy, -300)]
        [TestCase(FundingSourceType.CoInvestedSfa, 300)]
        [TestCase(FundingSourceType.CoInvestedSfa, -300)]
        [TestCase(FundingSourceType.CoInvestedEmployer, 300)]
        [TestCase(FundingSourceType.CoInvestedEmployer, -300)]
        public void Built_Event_Carries_Course_Type(FundingSourceType fundingSourceType, int amount)
        {
            var fundingSourceEvent = BuildEvent(fundingSourceType, amount);

            fundingSourceEvent.CourseType.Should().Be(CourseType.ShortCourse);
        }

        [TestCase(FundingSourceType.Levy, 300)]
        [TestCase(FundingSourceType.Levy, -300)]
        [TestCase(FundingSourceType.CoInvestedSfa, 300)]
        [TestCase(FundingSourceType.CoInvestedSfa, -300)]
        [TestCase(FundingSourceType.CoInvestedEmployer, 300)]
        [TestCase(FundingSourceType.CoInvestedEmployer, -300)]
        public void Built_Event_Keeps_Learning_Aim_Details_And_Amount(FundingSourceType fundingSourceType, int amount)
        {
            var fundingSourceEvent = BuildEvent(fundingSourceType, amount);

            fundingSourceEvent.LearningAim.LearningType.Should().Be(LearningType.ApprenticeshipUnit);
            fundingSourceEvent.LearningAim.CourseCode.Should().Be(ApprenticeshipUnitTestData.CourseCode);
            fundingSourceEvent.LearningAim.FundingLineType.Should().Be(ApprenticeshipUnitTestData.LevyFundingLineType);
            fundingSourceEvent.AmountDue.Should().Be(amount);
        }

        private static FundingSourcePaymentEvent BuildEvent(FundingSourceType fundingSourceType, int amount)
        {
            var processor = new Mock<IPaymentProcessor>();
            processor.Setup(x => x.Process(It.IsAny<RequiredPayment>()))
                .Returns(new List<FundingSourcePayment> { CreateFundingSourcePayment(fundingSourceType, amount) }.AsReadOnly());
            var autoMapper = AutoMapperConfigurationFactory.CreateMappingConfig().CreateMapper();
            var builder = new FundingSourcePaymentEventBuilder(autoMapper, processor.Object);
            var requiredPayment = ApprenticeshipUnitTestData.LevyRequiredPayment(ApprenticeshipEmployerType.Levy, amount);

            return builder.BuildFundingSourcePaymentsForRequiredPayment(requiredPayment, 1000000, 1).Single();
        }

        private static FundingSourcePayment CreateFundingSourcePayment(FundingSourceType fundingSourceType, decimal amount)
        {
            switch (fundingSourceType)
            {
                case FundingSourceType.Levy:
                    return new LevyPayment { AmountDue = amount, Type = FundingSourceType.Levy };
                case FundingSourceType.CoInvestedSfa:
                    return new SfaCoInvestedPayment { AmountDue = amount, Type = FundingSourceType.CoInvestedSfa };
                default:
                    return new EmployerCoInvestedPayment { AmountDue = amount, Type = FundingSourceType.CoInvestedEmployer };
            }
        }
    }
}
