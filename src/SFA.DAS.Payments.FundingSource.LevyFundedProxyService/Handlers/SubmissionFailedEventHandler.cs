﻿using System.Threading.Tasks;
using ESFA.DC.Logging.Interfaces;
using Microsoft.ServiceFabric.Actors.Client;
using SFA.DAS.Payments.Application.Infrastructure.Logging;
using SFA.DAS.Payments.FundingSource.Application.Interfaces;
using SFA.DAS.Payments.FundingSource.Application.Services;
using SFA.DAS.Payments.FundingSource.LevyFundedService.Interfaces;
using SFA.DAS.Payments.Monitoring.Jobs.DataMessages.Events;

namespace SFA.DAS.Payments.FundingSource.LevyFundedProxyService.Handlers
{
    public class SubmissionFailedEventHandler : SubmissionEventHandler<SubmissionJobFailed>
    {
        public SubmissionFailedEventHandler(IActorProxyFactory proxyFactory, ILevyFundingSourceRepository repository,
            IPaymentLogger logger, IExecutionContext executionContext,
            ILevyMessageRoutingService levyMessageRoutingService) : base(proxyFactory, repository, logger,
            executionContext, levyMessageRoutingService)
        {
        }

        protected override async Task HandleSubmissionEvent(SubmissionJobFailed message,
            ILevyFundedService fundingService)
        {
            await fundingService.RemoveCurrentSubmission(message);
        }
    }
}