﻿using System.Collections.Generic;
using SFA.DAS.Payments.FundingSource.Domain.Models;
namespace SFA.DAS.Payments.FundingSource.Domain.Interface
{
    public interface IValidateRequiredPaymentEvent
    {
        IEnumerable<RequiredPaymentEventValidationResult> Validate(CoInvestedPayment requiredPayment);
    }
}