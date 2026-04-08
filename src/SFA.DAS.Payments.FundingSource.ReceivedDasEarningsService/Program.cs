
namespace SFA.DAS.Payments.FundingSource.ReceivedDasEarningsService
{
    using System;
    using System.Threading;
    using global::SFA.DAS.Payments.ServiceFabric.Core.Infrastructure.Ioc;
    using SFA.DAS.Payments.ServiceFabric.Core.Infrastructure.Ioc;

    internal static class Program
    {
        private static void Main()
        {
            try
            {
                using (ServiceFabricContainerFactory.CreateContainerForActor<ReceivedDasEarningsService>())
                {
                    Thread.Sleep(Timeout.Infinite);
                }
            }
            catch (Exception e)
            {
                ActorEventSource.Current.ActorHostInitializationFailed(e.ToString());
                throw;
            }
        }
    }


}
