using System.Collections.Concurrent;
using NServiceBus;
using SFA.DAS.Payments.FundingSource.Messages.Events;
using SFA.DAS.Payments.FundingSource.Tests.Specs.Models;

namespace SFA.DAS.Payments.FundingSource.Tests.Specs.Handlers
{
    public class FundingSourcePaymentEventHandler : IHandleMessages<FundingSourcePaymentEvent>
    {
        public static ConcurrentBag<FundingSourcePaymentEvent> ReceivedEvents { get; } = new ConcurrentBag<FundingSourcePaymentEvent>();       
        public Task Handle(FundingSourcePaymentEvent message, IMessageHandlerContext context)
        {
            Console.WriteLine($"Received funding source payment event: {message.Ukprn}, {message.Learner.Uln}, {message.CollectionPeriod.AcademicYear}-{message.CollectionPeriod.Period}, {message.AmountDue}, {message.GetType().FullName}");
            ReceivedEvents.Add(message);
            return Task.CompletedTask;
        }

        public static IEnumerable<FundingSourcePaymentEvent> GetEvents(Learner learner) => ReceivedEvents.Where(receivedEvent =>
            receivedEvent.Learner.Uln == learner.Uln
            && receivedEvent.Ukprn == learner.Ukprn
            && receivedEvent.Learner.ReferenceNumber == learner.LearnRefNumber);
    }
}
