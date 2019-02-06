﻿using System.Collections.ObjectModel;
using System.Threading.Tasks;
using SFA.DAS.Payments.FundingSource.Messages.Events;
using SFA.DAS.Payments.RequiredPayments.Messages.Events;

namespace SFA.DAS.Payments.FundingSource.Application.Interfaces
{
    public interface IContractType1RequiredPaymentEventFundingSourceService
    {
        Task AddRequiredPayment(ApprenticeshipContractType1RequiredPaymentEvent paymentEvent);
        Task<ReadOnlyCollection<FundingSourcePaymentEvent>> GetFundedPayments(long employerAccountId);
    }
}