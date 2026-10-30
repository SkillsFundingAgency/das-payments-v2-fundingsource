using AutoMapper;
using FluentAssertions;
using NUnit.Framework;
using SFA.DAS.Payments.FundingSource.Application.Infrastructure.Configuration;
using SFA.DAS.Payments.FundingSource.Messages.Events;
using SFA.DAS.Payments.Model.Core.Entities;

namespace SFA.DAS.Payments.FundingSource.Application.UnitTests.Mapper
{
    [TestFixture]
    public class ApprenticeshipUnitRequiredPaymentMappingTests
    {
        private IMapper autoMapper;

        [SetUp]
        public void SetUp()
        {
            autoMapper = AutoMapperConfigurationFactory.CreateMappingConfig().CreateMapper();
        }

        [TestCase(ApprenticeshipEmployerType.Levy)]
        [TestCase(ApprenticeshipEmployerType.NonLevy)]
        public void Levy_Required_Payment_Maps_Course_Type_To_Levy_Event(ApprenticeshipEmployerType employerType)
        {
            var requiredPayment = ApprenticeshipUnitTestData.LevyRequiredPayment(employerType);

            var fundingSourceEvent = autoMapper.Map<LevyFundingSourcePaymentEvent>(requiredPayment);

            fundingSourceEvent.CourseType.Should().Be(CourseType.ShortCourse);
        }

        [TestCase(ApprenticeshipEmployerType.Levy)]
        [TestCase(ApprenticeshipEmployerType.NonLevy)]
        public void Levy_Required_Payment_Maps_Course_Type_To_Transfer_Event(ApprenticeshipEmployerType employerType)
        {
            var requiredPayment = ApprenticeshipUnitTestData.LevyRequiredPayment(employerType);

            var fundingSourceEvent = autoMapper.Map<TransferFundingSourcePaymentEvent>(requiredPayment);

            fundingSourceEvent.CourseType.Should().Be(CourseType.ShortCourse);
        }

        [TestCase(ApprenticeshipEmployerType.Levy)]
        [TestCase(ApprenticeshipEmployerType.NonLevy)]
        public void Levy_Required_Payment_Maps_Course_Type_To_Employer_Co_Invested_Event(ApprenticeshipEmployerType employerType)
        {
            var requiredPayment = ApprenticeshipUnitTestData.LevyRequiredPayment(employerType);

            var fundingSourceEvent = autoMapper.Map<EmployerCoInvestedFundingSourcePaymentEvent>(requiredPayment);

            fundingSourceEvent.CourseType.Should().Be(CourseType.ShortCourse);
        }

        [TestCase(ApprenticeshipEmployerType.Levy)]
        [TestCase(ApprenticeshipEmployerType.NonLevy)]
        public void Levy_Required_Payment_Maps_Course_Type_To_Sfa_Co_Invested_Event(ApprenticeshipEmployerType employerType)
        {
            var requiredPayment = ApprenticeshipUnitTestData.LevyRequiredPayment(employerType);

            var fundingSourceEvent = autoMapper.Map<SfaCoInvestedFundingSourcePaymentEvent>(requiredPayment);

            fundingSourceEvent.CourseType.Should().Be(CourseType.ShortCourse);
        }

        [TestCase(ApprenticeshipEmployerType.Levy)]
        [TestCase(ApprenticeshipEmployerType.NonLevy)]
        public void Levy_Required_Payment_Maps_Course_Type_To_Sfa_Fully_Funded_Event(ApprenticeshipEmployerType employerType)
        {
            var requiredPayment = ApprenticeshipUnitTestData.LevyRequiredPayment(employerType);

            var fundingSourceEvent = autoMapper.Map<SfaFullyFundedFundingSourcePaymentEvent>(requiredPayment);

            fundingSourceEvent.CourseType.Should().Be(CourseType.ShortCourse);
        }

        [TestCase(ApprenticeshipEmployerType.Levy)]
        [TestCase(ApprenticeshipEmployerType.NonLevy)]
        public void Co_Invested_Required_Payment_Maps_Course_Type_To_Employer_Co_Invested_Event(ApprenticeshipEmployerType employerType)
        {
            var requiredPayment = ApprenticeshipUnitTestData.CoInvestedRequiredPayment(employerType);

            var fundingSourceEvent = autoMapper.Map<EmployerCoInvestedFundingSourcePaymentEvent>(requiredPayment);

            fundingSourceEvent.CourseType.Should().Be(CourseType.ShortCourse);
        }

        [TestCase(ApprenticeshipEmployerType.Levy)]
        [TestCase(ApprenticeshipEmployerType.NonLevy)]
        public void Co_Invested_Required_Payment_Maps_Course_Type_To_Sfa_Co_Invested_Event(ApprenticeshipEmployerType employerType)
        {
            var requiredPayment = ApprenticeshipUnitTestData.CoInvestedRequiredPayment(employerType);

            var fundingSourceEvent = autoMapper.Map<SfaCoInvestedFundingSourcePaymentEvent>(requiredPayment);

            fundingSourceEvent.CourseType.Should().Be(CourseType.ShortCourse);
        }

        [TestCase(ApprenticeshipEmployerType.Levy)]
        [TestCase(ApprenticeshipEmployerType.NonLevy)]
        public void Co_Invested_Required_Payment_Maps_Course_Type_To_Sfa_Fully_Funded_Event(ApprenticeshipEmployerType employerType)
        {
            var requiredPayment = ApprenticeshipUnitTestData.CoInvestedRequiredPayment(employerType);

            var fundingSourceEvent = autoMapper.Map<SfaFullyFundedFundingSourcePaymentEvent>(requiredPayment);

            fundingSourceEvent.CourseType.Should().Be(CourseType.ShortCourse);
        }

        [TestCase(ApprenticeshipEmployerType.Levy)]
        [TestCase(ApprenticeshipEmployerType.NonLevy)]
        public void Levy_Required_Payment_Keeps_Learning_Aim_Details_On_Levy_Event(ApprenticeshipEmployerType employerType)
        {
            var requiredPayment = ApprenticeshipUnitTestData.LevyRequiredPayment(employerType);

            var fundingSourceEvent = autoMapper.Map<LevyFundingSourcePaymentEvent>(requiredPayment);

            fundingSourceEvent.LearningAim.LearningType.Should().Be(LearningType.ApprenticeshipUnit);
            fundingSourceEvent.LearningAim.CourseCode.Should().Be(ApprenticeshipUnitTestData.CourseCode);
            fundingSourceEvent.LearningAim.FundingLineType.Should().Be(ApprenticeshipUnitTestData.FundingLineTypeFor(employerType));
        }

        [TestCase(ApprenticeshipEmployerType.Levy)]
        [TestCase(ApprenticeshipEmployerType.NonLevy)]
        public void Co_Invested_Required_Payment_Keeps_Learning_Aim_Details_On_Sfa_Co_Invested_Event(ApprenticeshipEmployerType employerType)
        {
            var requiredPayment = ApprenticeshipUnitTestData.CoInvestedRequiredPayment(employerType);

            var fundingSourceEvent = autoMapper.Map<SfaCoInvestedFundingSourcePaymentEvent>(requiredPayment);

            fundingSourceEvent.LearningAim.LearningType.Should().Be(LearningType.ApprenticeshipUnit);
            fundingSourceEvent.LearningAim.CourseCode.Should().Be(ApprenticeshipUnitTestData.CourseCode);
            fundingSourceEvent.LearningAim.FundingLineType.Should().Be(ApprenticeshipUnitTestData.FundingLineTypeFor(employerType));
        }

        [TestCase(ApprenticeshipEmployerType.Levy)]
        [TestCase(ApprenticeshipEmployerType.NonLevy)]
        public void Co_Invested_Required_Payment_Keeps_Employer_Type_On_Sfa_Co_Invested_Event(ApprenticeshipEmployerType employerType)
        {
            var requiredPayment = ApprenticeshipUnitTestData.CoInvestedRequiredPayment(employerType);

            var fundingSourceEvent = autoMapper.Map<SfaCoInvestedFundingSourcePaymentEvent>(requiredPayment);

            fundingSourceEvent.ApprenticeshipEmployerType.Should().Be(employerType);
        }
    }
}
