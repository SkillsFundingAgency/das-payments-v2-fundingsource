using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Storage;
using SFA.DAS.Payments.Application.Infrastructure.Logging;
using SFA.DAS.Payments.Core;
using SFA.DAS.Payments.FundingSource.Application.Data;
using SFA.DAS.Payments.Model.Core.Entities;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace SFA.DAS.Payments.FundingSource.Application.Repositories
{
    public interface ILevyTransactionRepository
    {
        Task SaveLevyTransactions(IList<LevyTransactionModel> levyTransactions, CancellationToken cancellationToken);

        Task SaveLevyTransactionsIndividually(IList<LevyTransactionModel> levyTransactions, CancellationToken cancellationToken);

        Task<LevyTransactionModel> GetLevyTransactionAsync(string CourseCode, short AcademicYear, byte Period, long UKPRN, long ULN, string LearningAimReference);

        Task DeleteLevyTransaction(LevyTransactionModel levyTransactionModel, CancellationToken cancellationToken);

    }

    public class LevyTransactionRepository : ILevyTransactionRepository
    {
        private readonly IPaymentLogger logger;
        private readonly IFundingSourceDataContextFactory dataContextFactory;

        public LevyTransactionRepository(IFundingSourceDataContextFactory dataContextFactory, IPaymentLogger logger)
        {
            this.dataContextFactory = dataContextFactory ?? throw new ArgumentNullException(nameof(dataContextFactory));
            this.logger = logger ?? throw new ArgumentNullException(nameof(logger));
        }

        public async Task DeleteLevyTransaction(LevyTransactionModel levyTransactionModel, CancellationToken cancellationToken)
        {
            using var context = (FundingSourceDataContext)dataContextFactory.Create();
            context.ChangeTracker.AutoDetectChangesEnabled = false;
            context.LevyTransactions.Remove(levyTransactionModel);
            await context.SaveChangesAsync(cancellationToken).ConfigureAwait(false);
        }

        public async Task<LevyTransactionModel> GetLevyTransactionAsync(string courseCode, short academicYear, byte period, long ukprn, long uln, string learningAimReference)
        {
            using var context = (FundingSourceDataContext)dataContextFactory.Create();
            var levyTransactions = await context.LevyTransactions
                        .AsNoTracking()
                        .Where(x =>
                            x.Ukprn == ukprn
                            && x.AcademicYear == academicYear
                            && x.CourseCode == courseCode
                            && x.CollectionPeriod == period
                            && x.LearnerUln == uln
                            && x.LearningAimReference == learningAimReference)
                            .Select(x => new LevyTransactionModel
                            {
                                CourseCode = x.CourseCode,
                                Ukprn = x.Ukprn,
                                LearnerUln = x.LearnerUln,
                                AcademicYear = x.AcademicYear,
                                CollectionPeriod = x.CollectionPeriod,
                                LearningAimReference = x.LearningAimReference
                            })
                        .FirstOrDefaultAsync();

            return levyTransactions;
        }

        public async Task SaveLevyTransactions(IList<LevyTransactionModel> levyTransactions, CancellationToken cancellationToken)
        {
            using (var context = (FundingSourceDataContext)dataContextFactory.Create())
            {
                context.ChangeTracker.AutoDetectChangesEnabled = false;
                await context.LevyTransactions.AddRangeAsync(levyTransactions, cancellationToken).ConfigureAwait(false);
                await context.SaveChangesAsync(cancellationToken).ConfigureAwait(false);
            }
        }

        public async Task SaveLevyTransactionsIndividually(IList<LevyTransactionModel> levyTransactions, CancellationToken cancellationToken)
        {
            var mainContext = (FundingSourceDataContext)dataContextFactory.Create();

            using (var mainTransaction = await mainContext.Database
                .BeginTransactionAsync(IsolationLevel.ReadUncommitted, cancellationToken)
                .ConfigureAwait(false))
            {
                foreach (var model in levyTransactions)
                {
                    try
                    {
                        model.Id = 0;
                        var context = (FundingSourceDataContext)dataContextFactory.Create(mainTransaction.GetDbTransaction());
                        await context.LevyTransactions.AddAsync(model, cancellationToken).ConfigureAwait(false);
                        await context.SaveChangesAsync(cancellationToken).ConfigureAwait(false);
                    }
                    catch (Exception e)
                    {
                        if (e.IsUniqueKeyConstraintException())
                        {
                            logger.LogWarning($"Discarding duplicate LevyTransaction. JobId: {model.JobId}, Learn ref: {model.LearnerReferenceNumber}");
                            continue;
                        }
                        throw;
                    }
                }

                mainTransaction.Commit();
            }

        }
    }
}