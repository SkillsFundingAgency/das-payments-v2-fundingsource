using AutoMapper;
using FluentAssertions;
using Newtonsoft.Json;
using NUnit.Framework;
using SFA.DAS.Payments.FundingSource.Application.Infrastructure.Configuration;
using SFA.DAS.Payments.FundingSource.Messages.Events;
using SFA.DAS.Payments.Model.Core.Entities;
using SFA.DAS.Payments.RequiredPayments.Messages.Events;

namespace SFA.DAS.Payments.FundingSource.Application.UnitTests.Mapper
{
    [TestFixture]
    public class ApprenticeshipUnitRequiredPaymentDeserialisationTests
    {
        private const string CoInvestedClawbackJson =
            "{\"CourseType\":3,\"ApprenticeshipEmployerType\":0,\"ContractType\":1,\"AmountDue\":-300.0," +
            "\"LearningAim\":{\"Reference\":\"ZSC0001\",\"CourseCode\":\"ZSC0001\",\"LearningType\":4,\"FundingLineType\":\"GSO Short Courses (Apprenticeship Units) Non-Levy\"}," +
            "\"Learner\":{\"Uln\":1000000001,\"ReferenceNumber\":\"LEARNER1\"}," +
            "\"CollectionPeriod\":{\"AcademicYear\":2627,\"Period\":2}}";

        private IMapper autoMapper;

        [SetUp]
        public void SetUp()
        {
            autoMapper = AutoMapperConfigurationFactory.CreateMappingConfig().CreateMapper();
        }

        [Test]
        public void Co_Invested_Required_Payment_Json_Deserialises_Course_Type()
        {
            var requiredPayment = JsonConvert.DeserializeObject<CalculatedRequiredCoInvestedAmount>(CoInvestedClawbackJson);

            requiredPayment.CourseType.Should().Be(CourseType.ShortCourse);
        }

        [Test]
        public void Deserialised_Co_Invested_Required_Payment_Maps_Course_Type_To_Sfa_Co_Invested_Event()
        {
            var requiredPayment = JsonConvert.DeserializeObject<CalculatedRequiredCoInvestedAmount>(CoInvestedClawbackJson);

            var fundingSourceEvent = autoMapper.Map<SfaCoInvestedFundingSourcePaymentEvent>(requiredPayment);

            fundingSourceEvent.CourseType.Should().Be(CourseType.ShortCourse);
        }

        [Test]
        public void Deserialised_Co_Invested_Required_Payment_Maps_Course_Type_To_Employer_Co_Invested_Event()
        {
            var requiredPayment = JsonConvert.DeserializeObject<CalculatedRequiredCoInvestedAmount>(CoInvestedClawbackJson);

            var fundingSourceEvent = autoMapper.Map<EmployerCoInvestedFundingSourcePaymentEvent>(requiredPayment);

            fundingSourceEvent.CourseType.Should().Be(CourseType.ShortCourse);
        }
    }
}
