using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Azure.Amqp.Framing;
using Microsoft.ServiceFabric.Actors;
using Microsoft.ServiceFabric.Actors.Runtime;
using Microsoft.ServiceFabric.Actors.Client;
using SFA.DAS.Payments.EarningEvents.Messages.Events;
using SFA.DAS.Payments.FundingSource.Application.Repositories;
using SFA.DAS.Payments.Application.Infrastructure.Logging;
using SFA.DAS.Payments.FundingSource.Application.Interfaces;
using SFA.DAS.Payments.FundingSource.Application.Services;

namespace SFA.DAS.Payments.FundingSource.ReceivedDasEarningsService
{
    /// <remarks>
    /// This class represents an actor.
    /// Every ActorID maps to an instance of this class.
    /// The StatePersistence attribute determines persistence and replication of actor state:
    ///  - Persisted: State is written to disk and replicated.
    ///  - Volatile: State is kept in memory only and replicated.
    ///  - None: State is kept in memory only and not replicated.
    /// </remarks>
    [StatePersistence(StatePersistence.Persisted)]
    public class ReceivedDasEarningsService : Actor, IReceivedDasEarningsService
    {
        private readonly IReceivedDasEarningsProcessor dasEarningsProcessor;
        
        /// <summary>
        /// Initializes a new instance of ReceivedDasEarningsService
        /// </summary>
        /// <param name="actorService">The Microsoft.ServiceFabric.Actors.Runtime.ActorService that will host this actor instance.</param>
        /// <param name="actorId">The Microsoft.ServiceFabric.Actors.ActorId for this actor instance.</param>
        /// <param name="dasEarningsProcessor">Processor for message indicating that new earnings have been received from DAS</param>
        public ReceivedDasEarningsService(ActorService actorService, ActorId actorId, IReceivedDasEarningsProcessor dasEarningsProcessor) 
            : base(actorService, actorId)
        {
            this.dasEarningsProcessor = dasEarningsProcessor;
        }
        
        /// <summary>
        /// This method is called whenever an actor is activated.
        /// An actor is activated the first time any of its methods are invoked.
        /// </summary>
        protected override Task OnActivateAsync()
        {
            ActorEventSource.Current.ActorMessage(this, "Actor activated.");

            // The StateManager is this actor's private state store.
            // Data stored in the StateManager will be replicated for high-availability for actors that use volatile or persisted state storage.
            // Any serializable object can be saved in the StateManager.
            // For more information, see https://aka.ms/servicefabricactorsstateserialization

            return this.StateManager.TryAddStateAsync("count", 0);
        }

        public async Task RemovePreviousEarningsInCurrentCollection(DasEarningsReceivedEvent message, CancellationToken cancellationToken)
        {
            await dasEarningsProcessor.RemovePreviousEarningsInCurrentCollection(message, cancellationToken);
        }
    }
}
