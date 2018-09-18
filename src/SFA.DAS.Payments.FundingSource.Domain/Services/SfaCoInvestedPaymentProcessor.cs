﻿using SFA.DAS.Payments.FundingSource.Domain.Interface;
using SFA.DAS.Payments.FundingSource.Domain.Models;

namespace SFA.DAS.Payments.FundingSource.Domain.Services
{
    public class SfaCoInvestedPaymentProcessor : CoInvestedPaymentProcessor
    {

        public SfaCoInvestedPaymentProcessor(IValidateRequiredPaymentEvent validateRequiredPaymentEvent)
            : base(validateRequiredPaymentEvent)
        {
        }

        protected override CoInvestedPayment CreatePayment(RequiredCoInvestedPayment message)
        {
            var amountToPay = message.SfaContributionPercentage * message.AmountDue;

            return new SfaCoInvestedPayment
            {
                AmountDue = amountToPay,
                 Type = Enum.FundingSourceType.CoInvestedSfa,
                 
            };
        }
    }
}