﻿using System;
using System.Threading;
using System.Threading.Tasks;
using SFA.DAS.Payments.Application.Infrastructure.Logging;

namespace SFA.DAS.Payments.FundingSource.Application.Services
{
    public interface IProcessLevyAccountBalanceService
    {
        Task RefreshLevyAccountDetails(CancellationToken cancellationToken = default(CancellationToken));
    }

    public class ProcessLevyAccountBalanceService : IProcessLevyAccountBalanceService
    {
        private readonly IManageLevyAccountBalanceService accountBalanceService;
        private readonly IPaymentLogger paymentLogger;

        public ProcessLevyAccountBalanceService(IManageLevyAccountBalanceService accountBalanceService, IPaymentLogger paymentLogger)
        {
            this.accountBalanceService = accountBalanceService;
            this.paymentLogger = paymentLogger;
        }

        public async Task RefreshLevyAccountDetails(CancellationToken cancellationToken = default(CancellationToken))
        {
            try
            {
                paymentLogger.LogInfo("Starting to refresh all Levy Accounts Details");

                try
                {
                    await accountBalanceService.RefreshLevyAccountDetails(cancellationToken);
                }
                catch (Exception e)
                {
                    paymentLogger.LogError("Error While trying to refresh all Levy Accounts Details", e);
                }

            }
            catch (TaskCanceledException e)
            {
                paymentLogger.LogError("Levy Accounts Refresh Task was Canceled", e);
            }
            catch (Exception e)
            {
                paymentLogger.LogError("Error While trying to refresh all Levy Accounts Details", e);
                throw;
            }
        }
    }
}
