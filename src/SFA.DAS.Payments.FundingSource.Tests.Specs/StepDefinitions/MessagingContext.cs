using NServiceBus;

namespace SFA.DAS.Payments.FundingSource.Tests.Specs.StepDefinitions
{
    public class MessagingContext
    {
        private IEndpointInstance endpointInstance;

        public MessagingContext()
        {
            endpointInstance = TestRunBindings.Endpoint;            
        }
        
        public async Task Send<T>(T eventMessage)
        {
            await endpointInstance.Send("sfa-das-payments-fundingsource-levy", eventMessage);
        }

        public async Task SendToLevyTransaction<T>(T eventMessage)
        {
            await endpointInstance.Send("sfa-das-payments-fundingsource-levy-transaction", eventMessage);
        }

        public async Task SendToNonLevy<T>(T eventMessage)
        {
            await endpointInstance.Send("sfa-das-payments-fundingsource-nonlevy", eventMessage);
        }
    }
}
