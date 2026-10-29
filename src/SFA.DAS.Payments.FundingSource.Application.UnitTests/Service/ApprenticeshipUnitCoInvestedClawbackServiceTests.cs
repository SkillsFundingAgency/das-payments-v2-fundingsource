using System.Collections.Generic;
using System.Linq;
using FluentAssertions;
using Moq;
using NUnit.Framework;
using SFA.DAS.Payments.FundingSource.Application.Infrastructure.Configuration;
using SFA.DAS.Payments.FundingSource.Application.Services;
using SFA.DAS.Payments.FundingSource.Application.UnitTests.Mapper;
using SFA.DAS.Payments.FundingSource.Domain.Interface;
using SFA.DAS.Payments.FundingSource.Domain.Models;
using SFA.DAS.Payments.FundingSource.Messages.Events;
using SFA.DAS.Payments.Model.Core.Entities;

namespace SFA.DAS.Payments.FundingSource.Application.UnitTests.Service
{
    [TestFixture]
    public class ApprenticeshipUnitCoInvestedClawbackServiceTests
    {
        private CoInvestedFundingSourceService service;

        [SetUp]
        public void SetUp()
        {
            var sfaProcessor = new Mock<ICoInvestedPaymentProcessorOld>();
            sfaProcessor.Setup(x => x.Process(It.IsAny<RequiredPayment>()))
                .Returns(new SfaCoInvestedPayment { AmountDue = -285m, Type = FundingSourceType.CoInvestedSfa });

            var employerProcessor = new Mock<ICoInvestedPaymentProcessorOld>();
            employerProcessor.Setup(x => x.Process(It.IsAny<RequiredPayment>()))
                .Returns(new EmployerCoInvestedPayment { AmountDue = -15m, Type = FundingSourceType.CoInvestedEmployer });

            var autoMapper = AutoMapperConfigurationFactory.CreateMappingConfig().CreateMapper();
            service = new CoInvestedFundingSourceService(
                new List<ICoInvestedPaymentProcessorOld> { sfaProcessor.Object, employerProcessor.Object },
                new CoInvestedFundingSourcePaymentEventMapper(autoMapper));
        }

        [TestCase(ApprenticeshipEmployerType.Levy)]
        [TestCase(ApprenticeshipEmployerType.NonLevy)]
        public void Clawback_Is_Split_Into_Sfa_And_Employer_Funding_Source_Events(ApprenticeshipEmployerType employerType)
        {
            var events = GetClawbackEvents(employerType);

            events.Should().HaveCount(2);
            events.Should().ContainSingle(x => x is SfaCoInvestedFundingSourcePaymentEvent && x.AmountDue == -285m);
            events.Should().ContainSingle(x => x is EmployerCoInvestedFundingSourcePaymentEvent && x.AmountDue == -15m);
        }

        [TestCase(ApprenticeshipEmployerType.Levy)]
        [TestCase(ApprenticeshipEmployerType.NonLevy)]
        public void Clawback_Events_Carry_Course_Type(ApprenticeshipEmployerType employerType)
        {
            var events = GetClawbackEvents(employerType);

            events.Should().OnlyContain(x => x.CourseType == CourseType.ShortCourse);
        }

        [TestCase(ApprenticeshipEmployerType.Levy)]
        [TestCase(ApprenticeshipEmployerType.NonLevy)]
        public void Clawback_Events_Keep_Learning_Aim_Details(ApprenticeshipEmployerType employerType)
        {
            var events = GetClawbackEvents(employerType);

            events.Should().OnlyContain(x => x.LearningAim.LearningType == LearningType.ApprenticeshipUnit);
            events.Should().OnlyContain(x => x.LearningAim.CourseCode == ApprenticeshipUnitTestData.CourseCode);
            events.Should().OnlyContain(x => x.LearningAim.FundingLineType == ApprenticeshipUnitTestData.FundingLineTypeFor(employerType));
        }

        private List<CoInvestedFundingSourcePaymentEvent> GetClawbackEvents(ApprenticeshipEmployerType employerType)
        {
            var requiredPayment = ApprenticeshipUnitTestData.CoInvestedRequiredPayment(employerType);
            return service.GetFundedPayments(requiredPayment).ToList();
        }
    }
}
