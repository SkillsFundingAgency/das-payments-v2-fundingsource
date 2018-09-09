﻿using Moq;
using NUnit.Framework;
using SFA.DAS.Payments.FundingSource.Domain.Exceptions;
using SFA.DAS.Payments.FundingSource.Domain.Interface;
using SFA.DAS.Payments.FundingSource.Domain.Models;
using SFA.DAS.Payments.FundingSource.Domain.Services;
using SFA.DAS.Payments.RequiredPayments.Messages.Events;
using System.Collections.Generic;

namespace SFA.DAS.Payments.FundingSource.Domain.UnitTests
{
    [TestFixture]
    public class EmployerCoInvestedPaymentProcessorTest
    {
        private EmployerCoInvestedPaymentProcessor processor;
        private Mock<IValidateRequiredPaymentEvent> validator;

        [Test]
        public void ShouldThrowExceptionIfValidationResultIsNotNull()
        {
            var message = new ApprenticeshipContractType2RequiredPaymentEvent
            {
                SfaContributionPercentage = 0
            };

            var validationResults = new List<RequiredPaymentEventValidationResult>
            {
                new RequiredPaymentEventValidationResult
                {
                    RequiredPaymentEventMesage = message,
                    Rule = RequiredPaymentEventValidationRules.ZeroSfaContributionPercentage
                }
            };

            validator = new Mock<IValidateRequiredPaymentEvent>();
            validator.Setup(o => o.Validate(message)).Returns(validationResults);

            processor = new EmployerCoInvestedPaymentProcessor(validator.Object);

            Assert.That(() => processor.Process(message), Throws.InstanceOf<FundingSourceRequiredPaymentValidationException>());
        }

        [TestCase(0.9, 20600.87, 2060.087)]
        [TestCase(0.9, 552580.20, 55258.02)]
        [TestCase(1, 552580.20, 0)]
        public void GivenValidSfaContributionAndAmountDueSholudReturnValidPayment(decimal sfaContribution,
                                                                                   decimal amountDue,
                                                                                   decimal expectedAmount)
        {
            var message = new ApprenticeshipContractType2RequiredPaymentEvent
            {
                SfaContributionPercentage = sfaContribution,
                AmountDue = amountDue,
                JobId = "H001"
            };
            validator = new Mock<IValidateRequiredPaymentEvent>();
            validator.Setup(o => o.Validate(message)).Returns(new List<RequiredPaymentEventValidationResult>());
            processor = new EmployerCoInvestedPaymentProcessor(validator.Object);
            var payment = processor.Process(message);
            Assert.AreEqual(expectedAmount, payment.AmountDue);
        }
    }
}