using System;
using SFA.DAS.Payments.Model.Core;
using SFA.DAS.Payments.Model.Core.Entities;
using SFA.DAS.Payments.Model.Core.Factories;
using SFA.DAS.Payments.Model.Core.OnProgramme;
using SFA.DAS.Payments.RequiredPayments.Messages.Events;

namespace SFA.DAS.Payments.FundingSource.Application.UnitTests.Mapper
{
    internal static class ApprenticeshipUnitTestData
    {
        public const string LevyFundingLineType = "GSO Short Courses (Apprenticeship Units) Levy";
        public const string NonLevyFundingLineType = "GSO Short Courses (Apprenticeship Units) Non-Levy";
        public const string CourseCode = "ZSC0001";

        public static string FundingLineTypeFor(ApprenticeshipEmployerType employerType)
        {
            return employerType == ApprenticeshipEmployerType.Levy ? LevyFundingLineType : NonLevyFundingLineType;
        }

        public static CalculatedRequiredLevyAmount LevyRequiredPayment(ApprenticeshipEmployerType employerType, decimal amount = -300m)
        {
            var requiredPayment = new CalculatedRequiredLevyAmount
            {
                AgreementId = "AGREEMENT",
                FundingPlatformType = FundingPlatformType.DigitalApprenticeshipService
            };
            Populate(requiredPayment, employerType, amount);
            return requiredPayment;
        }

        public static CalculatedRequiredCoInvestedAmount CoInvestedRequiredPayment(ApprenticeshipEmployerType employerType, decimal amount = -300m)
        {
            var requiredPayment = new CalculatedRequiredCoInvestedAmount
            {
                FundingPlatformType = FundingPlatformType.DigitalApprenticeshipService
            };
            Populate(requiredPayment, employerType, amount);
            return requiredPayment;
        }

        private static void Populate(CalculatedRequiredOnProgrammeAmount requiredPayment, ApprenticeshipEmployerType employerType, decimal amount)
        {
            requiredPayment.EventId = Guid.NewGuid();
            requiredPayment.AmountDue = amount;
            requiredPayment.CollectionPeriod = CollectionPeriodFactory.CreateFromAcademicYearAndPeriod(2627, 2);
            requiredPayment.DeliveryPeriod = 1;
            requiredPayment.Learner = new Learner { ReferenceNumber = "LEARNER1", Uln = 1000000001 };
            requiredPayment.Ukprn = 10000001;
            requiredPayment.AccountId = 1000000;
            requiredPayment.ContractType = ContractType.Act1;
            requiredPayment.ApprenticeshipEmployerType = employerType;
            requiredPayment.OnProgrammeEarningType = OnProgrammeEarningType.Milestone1;
            requiredPayment.SfaContributionPercentage = 0.95m;
            requiredPayment.CourseType = CourseType.ShortCourse;
            requiredPayment.LearningAim = new LearningAim
            {
                Reference = CourseCode,
                CourseCode = CourseCode,
                LearningType = LearningType.ApprenticeshipUnit,
                FundingLineType = FundingLineTypeFor(employerType)
            };
        }
    }
}
