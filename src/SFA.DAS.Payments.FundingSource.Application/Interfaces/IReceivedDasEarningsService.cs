using Microsoft.ServiceFabric.Actors;
using SFA.DAS.Payments.EarningEvents.Messages.Events;
using System.Threading;
using System.Threading.Tasks;

namespace SFA.DAS.Payments.FundingSource.Application.Interfaces
{
    public interface IReceivedDasEarningsService: IActor
    {
        Task RemovePreviousEarningsInCurrentCollection(DasEarningsReceivedEvent message, CancellationToken cancellationToken);
    }
}