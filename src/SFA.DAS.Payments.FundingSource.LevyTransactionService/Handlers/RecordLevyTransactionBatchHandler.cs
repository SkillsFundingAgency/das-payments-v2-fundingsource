﻿using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using SFA.DAS.Payments.Application.Infrastructure.Logging;
using SFA.DAS.Payments.Application.Messaging;
using SFA.DAS.Payments.FundingSource.Application.Services;
using SFA.DAS.Payments.Monitoring.Jobs.Client;
using SFA.DAS.Payments.Monitoring.Jobs.DataMessages.Commands;
using SFA.DAS.Payments.RequiredPayments.Messages.Events;

namespace SFA.DAS.Payments.FundingSource.LevyTransactionService.Handlers
{
    public class RecordLevyTransactionBatchHandler : IHandleMessageBatches<CalculatedRequiredLevyAmount>
    {
        private readonly ILevyTransactionBatchStorageService levyTransactionBatchStorageService;
        private readonly IPaymentLogger logger;
        private readonly IJobMessageClientFactory monitoringClientFactory;

        public RecordLevyTransactionBatchHandler(IPaymentLogger logger,
            ILevyTransactionBatchStorageService levyTransactionBatchStorageService,
            IJobMessageClientFactory monitoringClientFactory)
        {
            this.logger = logger ?? throw new ArgumentNullException(nameof(logger));
            this.levyTransactionBatchStorageService = levyTransactionBatchStorageService ??
                                                      throw new ArgumentNullException(
                                                          nameof(levyTransactionBatchStorageService));
            this.monitoringClientFactory = monitoringClientFactory ??
                                           throw new ArgumentNullException(nameof(monitoringClientFactory));
        }

        public async Task Handle(IList<CalculatedRequiredLevyAmount> messages, CancellationToken cancellationToken)
        {
            logger.LogInfo($"Received {messages.Count} messages");
            await levyTransactionBatchStorageService.StoreLevyTransactions(messages, cancellationToken)
                .ConfigureAwait(false);

            var monitoringClient = monitoringClientFactory.Create();
            foreach (var calculatedRequiredLevyAmount in messages)
            {
                await monitoringClient.ProcessedJobMessage(calculatedRequiredLevyAmount.JobId,
                    calculatedRequiredLevyAmount.EventId,
                    calculatedRequiredLevyAmount.GetType().ToString(),
                    new List<GeneratedMessage>()
                );
            }
        }
    }
}