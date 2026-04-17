using SFA.DAS.Payments.EarningEvents.Messages.Events;
using SFA.DAS.Payments.FundingSource.Application.Repositories;
using System.Threading.Tasks;
using System.Threading;
using System;
using SFA.DAS.Payments.Application.Infrastructure.Logging;

namespace SFA.DAS.Payments.FundingSource.Application.Services
{
    public interface IReceivedDasEarningsProcessor
    {
        Task RemovePreviousEarningsInCurrentCollection(DasEarningsReceivedEvent message, CancellationToken cancellationToken);
    }

    public class ReceivedDasEarningsProcessor : IReceivedDasEarningsProcessor
    {
        private readonly ILevyTransactionRepository levyTransactionRepository;
        private readonly IPaymentLogger logger;

        public ReceivedDasEarningsProcessor(ILevyTransactionRepository repository, IPaymentLogger logger)
        {
            this.levyTransactionRepository = repository ?? throw new ArgumentNullException(nameof(repository));
            this.logger = logger ?? throw new ArgumentNullException(nameof(logger));
        }

        public async Task RemovePreviousEarningsInCurrentCollection(DasEarningsReceivedEvent message, CancellationToken cancellationToken)
        {
            ValidateDasEarningsReceivedEvent(message);

            var courseCode = message.CourseCode;
            var academicYear = message.CollectionPeriod.AcademicYear;
            var period = message.CollectionPeriod.Period;
            var ukprn = message.UKPRN;
            var uln = message.ULN;
            var learningAimReference = message.LearningAimReference;

            string logContext = $"CourseCode: {courseCode}, AcademicYear: {academicYear}, CollectionPeriod: {period}, UKPRN: {ukprn}, ULN: {uln}, LearningAimReference: {learningAimReference}";
            logger.LogInfo($"Looking in Levy Transactions table with {logContext}");

            try
            {
                var levyTransaction = await levyTransactionRepository.GetLevyTransactionAsync(courseCode, academicYear, period, ukprn, uln, learningAimReference).ConfigureAwait(false);

                if (levyTransaction is null)
                {
                    logger.LogInfo($"No Levy Transactions found for {logContext}");
                    return;
                }

                // If incoming earnings id is newer than stored one, remove the stored levy transaction(s)
                if (message.EarningsId.CompareTo(levyTransaction.EarningEventId) > 0)
                {
                    await levyTransactionRepository.DeleteLevyTransaction(levyTransaction, CancellationToken.None).ConfigureAwait(false);
                    logger.LogInfo($"Deleted levy transaction(s) for {logContext}");
                }
                else
                {
                    logger.LogInfo($"Existing levy transaction(s) are newer or equal for {logContext}. Message EarningsId: {message.EarningsId}, Stored EarningEventId: {levyTransaction.EarningEventId}");
                }
            }
            catch (Exception e)
            {
                logger.LogError($"Error while getting or deleting levy transactions for {logContext}", e);
                throw;
            }
        }

        private static void ValidateDasEarningsReceivedEvent(DasEarningsReceivedEvent message)
        {
            if (message == null)
            {
                throw new ArgumentNullException(nameof(message));
            }
            
            if (string.IsNullOrWhiteSpace(message.CourseCode))
            {
                throw new ArgumentException("CourseCode must be provided", nameof(message));
            }
            
            if (string.IsNullOrWhiteSpace(message.LearningAimReference))
            {
                throw new ArgumentException("LearningAimReference must be provided", nameof(message));
            }

            if (message.UKPRN == 0)
            {
                throw new ArgumentException("UKPRN must be provided", nameof(message));
            }

            if (message.ULN == 0)
            {
                throw new ArgumentException("ULN must be provided", nameof(message));
            }

            if (message.EarningsId == Guid.Empty)
            {
                throw new ArgumentException("EarningsId must be provided", nameof(message));
            }

            if (message.CollectionPeriod == null)
            {
                throw new ArgumentException("CollectionPeriod must be provided", nameof(message));
            }

            if (message.CollectionPeriod.AcademicYear == 0)
            {
                throw new ArgumentException("CollectionPeriod AcademicYear must be provided", nameof(message));
            }

            if (message.CollectionPeriod.Period == 0)
            {
                throw new ArgumentException("CollectionPeriod Period must be provided", nameof(message));
            }

            if (message.CollectionPeriod.Period > 14)
            {
                throw new ArgumentException("CollectionPeriod Period is invalid", nameof(message));
            }
        }
    }
}
