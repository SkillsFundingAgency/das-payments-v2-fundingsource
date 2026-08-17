using System;
using System.Collections.Generic;
using SFA.DAS.Payments.Core;
using SFA.DAS.Payments.FundingSource.Domain.Interface;
using SFA.DAS.Payments.FundingSource.Domain.Models;
using SFA.DAS.Payments.Model.Core.Entities;

namespace SFA.DAS.Payments.FundingSource.Domain.Services
{
    public class LevyPaymentProcessor : ILevyPaymentProcessor
    {
        private static readonly DateTime CoInvestmentRulesCutOffDate = new DateTime(2024, 4, 1);

        private readonly ILevyBalanceService levyBalanceService;

        public LevyPaymentProcessor(ILevyBalanceService levyBalanceService)
        {
            this.levyBalanceService = levyBalanceService;
        }

        public IReadOnlyList<FundingSourcePayment> Process(RequiredPayment requiredPayment)
        {
            // Learners with 100% SFA contribution who started before the co-investment rules cut-off are small employers and shouldn't draw on the levy balance.
            if (requiredPayment.SfaContributionPercentage == 1 && requiredPayment.StartDate < CoInvestmentRulesCutOffDate)
            {
                return new FundingSourcePayment[0];
            }

            var amountDue = levyBalanceService.TryFund(requiredPayment.AmountDue).AsRounded();

            if (amountDue == 0) 
                return new FundingSourcePayment[0];

            return new[]
            {
                new LevyPayment
                {
                    AmountDue = amountDue,
                    Type = FundingSourceType.Levy
                }
            };
        }
    }
}