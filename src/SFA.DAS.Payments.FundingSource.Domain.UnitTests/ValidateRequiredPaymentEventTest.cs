﻿using NUnit.Framework;
using SFA.DAS.Payments.FundingSource.Domain.Models;
using SFA.DAS.Payments.FundingSource.Domain.Services;
using System.Linq;
using NUnit.Framework.Legacy;

namespace SFA.DAS.Payments.FundingSource.Domain.UnitTests
{
    [TestFixture]
    public class ValidateRequiredPaymentEventTest
    {
        [Test]
        public void ShouldGenerateValidationResultForInvalidEvent()
        {
            var message = new RequiredCoInvestedPayment
            {
                SfaContributionPercentage = 0
            };

            var validator = new ValidateRequiredPaymentEvent();
            var results = validator.Validate(message);

            ClassicAssert.IsNotNull(results);
            ClassicAssert.AreEqual((int)RequiredPaymentEventValidationRules.ZeroSfaContributionPercentage, (int)results.First().Rule);
        }

    }
}
