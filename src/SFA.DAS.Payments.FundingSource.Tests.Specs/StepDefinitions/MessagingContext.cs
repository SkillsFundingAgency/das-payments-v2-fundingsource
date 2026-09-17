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
    }
}
