using NServiceBus;
using SFA.DAS.Payments.Application.Infrastructure.Logging;
using SFA.DAS.Payments.EarningEvents.Messages.Events;
using SFA.DAS.Payments.FundingSource.Application.Interfaces;
using System;
using System.Threading.Tasks;

namespace SFA.DAS.Payments.FundingSource.LevyFundedProxyService.Handlers
{
    public class ReceivedDASEarningsHandler : IHandleMessages<DasEarningsReceivedEvent>
    {
        private readonly IPaymentLogger logger;
        private readonly IReceivedDasEarningsService receivedDasEarningsService;

        public ReceivedDASEarningsHandler(IPaymentLogger logger, IReceivedDasEarningsService receivedDasEarningsService)
        {
            this.logger = logger ?? throw new ArgumentNullException(nameof(logger));
            this.receivedDasEarningsService = receivedDasEarningsService ?? throw new ArgumentNullException(nameof(receivedDasEarningsService));
        }

        public async Task Handle(DasEarningsReceivedEvent message, IMessageHandlerContext context)
        {
            if (message == null) throw new ArgumentNullException(nameof(message));
            if (context == null) throw new ArgumentNullException(nameof(context));

            var logContext = $"Message Id:{context.MessageId}, Earnings Id:{message.EarningsId}, CourseCode:{message.CourseCode}, CollectionPeriod:{message.CollectionPeriod?.Period}, UKPRN:{message.UKPRN}, ULN:{message.ULN}, LearningAimReference:{message.LearningAimReference}";

            logger.LogInfo($"Received DasEarningsReceived event. {logContext}");

            try
            {
                await receivedDasEarningsService.RemovePreviousEarningsInCurrentCollection(message).ConfigureAwait(false);
                logger.LogInfo($"Finished DasEarningsReceived event. {logContext}");
            }
            catch (Exception e)
            {
                logger.LogError($"Error processing DasEarningsReceived event. {logContext}", e);
                throw;
            }
        }
    }
}
