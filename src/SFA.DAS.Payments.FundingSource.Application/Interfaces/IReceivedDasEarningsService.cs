using SFA.DAS.Payments.EarningEvents.Messages.Events;
using System.Threading.Tasks;

namespace SFA.DAS.Payments.FundingSource.Application.Interfaces
{
    public interface IReceivedDasEarningsService
    {
        Task RemovePreviousEarningsInCurrentCollection(DasEarningsReceivedEvent message);
    }
}