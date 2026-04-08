using Microsoft.ServiceFabric.Actors;
using Microsoft.ServiceFabric.Actors.Client;
using NServiceBus;
using SFA.DAS.Payments.Application.Infrastructure.Logging;
using SFA.DAS.Payments.EarningEvents.Messages.Events;
using SFA.DAS.Payments.FundingSource.Application.Interfaces;
using System;
using System.Threading;
using System.Threading.Tasks;

namespace SFA.DAS.Payments.FundingSource.LevyFundedProxyService.Handlers
{
    public class ReceivedDASEarningsHandler : IHandleMessages<DasEarningsReceivedEvent>
    {
        private readonly IActorProxyFactory proxyFactory;
        private readonly IPaymentLogger logger;

        public ReceivedDASEarningsHandler(IPaymentLogger logger, IActorProxyFactory proxyFactory)
        {
            this.logger = logger ?? throw new ArgumentNullException(nameof(logger));
            this.proxyFactory = proxyFactory ?? throw new ArgumentNullException(nameof(proxyFactory));
        }

        public async Task Handle(DasEarningsReceivedEvent message, IMessageHandlerContext context)
        {
            if (message == null) throw new ArgumentNullException(nameof(message));
            if (context == null) throw new ArgumentNullException(nameof(context));

            var logContext = $"Message Id:{context.MessageId}, Earnings Id:{message.EarningsId}, CourseCode:{message.CourseCode}, CollectionPeriod:{message.CollectionPeriod?.Period}, UKPRN:{message.UKPRN}, ULN:{message.ULN}, LearningAimReference:{message.LearningAimReference}";

            logger.LogInfo($"Received DasEarningsReceived event. {logContext}");

            try
            {
                var actorId = new ActorId(message.EarningsId.ToString());
                var actor = proxyFactory.CreateActorProxy<IReceivedDasEarningsService>(new Uri("fabric:/SFA.DAS.Payments.FundingSource.ServiceFabric/ReceivedDasEarningsServiceActorService"), actorId);
                await actor.RemovePreviousEarningsInCurrentCollection(message, CancellationToken.None).ConfigureAwait(false);
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
