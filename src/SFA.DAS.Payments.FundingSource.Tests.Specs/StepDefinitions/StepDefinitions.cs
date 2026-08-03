using Bogus;
using SFA.DAS.Payments.Model.Core;
using SFA.DAS.Payments.Model.Core.Audit;
using SFA.DAS.Payments.Model.Core.Entities;
using SFA.DAS.Payments.Model.Core.OnProgramme;
using SFA.DAS.Payments.RequiredPayments.Messages.Events;
using System.Text.Json;
using Microsoft.EntityFrameworkCore;
using SFA.DAS.Payments.PeriodEnd.Messages.Events;

namespace SFA.DAS.Payments.FundingSource.Tests.Specs.StepDefinitions
{
    [Binding]
    public class StepDefinitions
    {
        protected readonly ScenarioContext scenarioContext;
        protected readonly MessagingContext messagingContext;
        protected readonly TestSession testSession;
        protected CollectionPeriod collectionPeriod;
        protected short currentAcademicYear;
        private Randomizer randomizer;
        private long employerAccountId;
        private decimal paymentAmount;
        private decimal sfaContributionPercentage;
        private ApprenticeshipEmployerType employerType;
        private LevyAccountModel levyAccount;
        private CalculatedRequiredLevyAmount calculatedRequiredLevyAmount;

        public StepDefinitions(ScenarioContext scenarioContext, MessagingContext messagingContext, TestSession testSession)
        {
            this.scenarioContext = scenarioContext;
            this.messagingContext = messagingContext;
            this.testSession = testSession;
        }

        protected void SetCurrentCollectionYear()
        {
            collectionPeriod = new CollectionPeriodBuilder().WithDate(DateTime.Today).Build();
            currentAcademicYear = collectionPeriod.AcademicYear;
        }

        [BeforeScenario]
        public void BeforeScenario()
        {
            randomizer = new Randomizer();
            employerAccountId = randomizer.Long(1, 100000);
            paymentAmount = 5000m;
            SetCurrentCollectionYear();
            Console.WriteLine($"UKPRN : {testSession.Provider.Ukprn}, ULN: {testSession.Learner.Uln}, collection year: {currentAcademicYear}");
        }

        [AfterScenario]
        public void AfterScenario()
        {
        }

        [Given("A Levy paying employer has sufficient funds in their Growth and Skills Levy account")]
        public async Task GivenALevyPayingEmployerHasSufficientFundsInTheirGrowthAndSkillsLevyAccount()
        {
            employerType = ApprenticeshipEmployerType.Levy;
            await InitialiseEmployerAccount(10000m, true);
        }

        [Given("A Levy paying employer has insufficient funds in their Growth and Skills Levy account")]
        public async Task GivenALevyPayingEmployerHasInsufficientFundsInTheirGrowthAndSkillsLevyAccount()
        {
            employerType = ApprenticeshipEmployerType.Levy;
            await InitialiseEmployerAccount(3000m, true);
        }

        [Given("A Levy paying employer has no funds in their Growth and Skills Levy account")]
        public async Task GivenALevyPayingEmployerHasNoFundsInTheirGrowthAndSkillsLevyAccount()
        {
            employerType = ApprenticeshipEmployerType.Levy;
            await InitialiseEmployerAccount(0m, true);
        }

        [Given("A Non-Levy paying employer has sufficient funds in their Growth and Skills Levy account")]
        public async Task GivenANonLevyPayingEmployerHasSufficientFundsInTheirGrowthAndSkillsLevyAccount()
        {
            employerType = ApprenticeshipEmployerType.NonLevy;
            await InitialiseEmployerAccount(10000m, false);
        }

        [Given("A Non-Levy paying employer has insufficient funds in their Growth and Skills Levy account")]
        public async Task GivenANonLevyPayingEmployerHasInsufficientFundsInTheirGrowthAndSkillsLevyAccount()
        {
            employerType = ApprenticeshipEmployerType.NonLevy;
            await InitialiseEmployerAccount(3000m, false);
        }

        [Given("A Non-Levy paying employer has no funds in their Growth and Skills Levy account")]
        public async Task GivenANonLevyPayingEmployerHasNoFundsInTheirGrowthAndSkillsLevyAccount()
        {
            employerType = ApprenticeshipEmployerType.NonLevy;
            await InitialiseEmployerAccount(0m, false);
        }

        [When("the SFA contribution percentage is set to 100% for the learner and course")]
        public async Task WhenTheSFAContributionPercentageIsSetToOneHundredPercentForTheLearnerAndCourse()
        {
            sfaContributionPercentage = 1m;
            await CreateApprenticeship();
            await GenerateFundingSourceLevyTransaction();
            await GenerateRequiredPaymentEvent();

            var periodEndEvent = new PeriodEndRunningEvent
            {
                CollectionPeriod = collectionPeriod,
                EventId = Guid.NewGuid(),
                EventTime = DateTimeOffset.Now,
                JobId = testSession.JobId
            };
            await messagingContext.Send(periodEndEvent);
        }

        [Then("the training provider payments should be fully funded by the employer's Growth and Skills Levy account")]
        public async Task ThenTheTrainingProviderPaymentsShouldBeFullyFundedByTheEmployersGrowthAndSkillsLevyAccount()
        {
            await testSession.WaitForIt(async () =>
            {
                var payments = GetPayments();

                if (payments.Count() != 1)
                    return false;
                
                var payment = payments.Single();
                if (payment.Amount != paymentAmount || payment.FundingSource != FundingSourceType.Levy)
                    return false;
                
                    
                return true;
            }, "Payment was not fully funded by employer levy");
        }

        [Then("the training provider payments should be partially funded by the employer's Growth and Skills Levy account")]
        public async Task ThenTheTrainingProviderPaymentsShouldBePartiallyFundedByTheEmployersGrowthAndSkillsLevyAccount()
        {
            await testSession.WaitForIt(async () =>
            {
                var payments = GetPayments();

                if (payments.Count() != 2)
                    return false;

                var levyPayment = payments.FirstOrDefault(x => x.FundingSource == FundingSourceType.Levy);
                if (levyPayment == null)
                    return false;

                if (levyPayment.Amount != levyAccount.Balance)
                    return false;

                var sfaPayment = payments.FirstOrDefault(x => x.FundingSource == FundingSourceType.CoInvestedSfa);
                if (sfaPayment == null)
                    return false;

                if (sfaPayment.Amount != (paymentAmount - levyAccount.Balance))
                    return false;

                return true;
            }, "Payment was not co funded by employer levy balance and SFA");
        }

        [Then("the training provider payments should be fully funded by SFA")]
        public async Task ThenTheTrainingProviderPaymentsShouldBeFullyFundedBySFA()
        {
            await testSession.WaitForIt(async () =>
            {
                var payments = GetPayments();

                if (payments.Count() != 1)
                    return false;

                var payment = payments.Single();
                if (payment.Amount != paymentAmount || payment.FundingSource != FundingSourceType.CoInvestedSfa)
                    return false;

                return true;
            }, "Payment was not fully funded by SFA contributions");
        }

        private IEnumerable<PaymentModel> GetPayments()
        {
            var payments = testSession.DataContext.Payment
                .Where(x => x.AccountId == employerAccountId
                    && x.Ukprn == testSession.Learner.Ukprn
                    && x.LearnerUln == testSession.Learner.Uln
                    && x.CollectionPeriod.Period == collectionPeriod.Period
                    && x.CollectionPeriod.AcademicYear == collectionPeriod.AcademicYear);

            return payments;
        }

        private async Task InitialiseEmployerAccount(decimal balance, bool isLevyPayer)
        {
            levyAccount = new LevyAccountModel
            {
                AccountId = employerAccountId,
                Balance = balance,
                IsLevyPayer = isLevyPayer,
                AccountName = "Automated Test Account",
                TransferAllowance = 0m
            };
            testSession.DataContext.LevyAccounts.Add(levyAccount);
            await testSession.DataContext.SaveChangesAsync();
        }

        private async Task CreateApprenticeship()
        {
            var apprenticeshipModelFaker = new Faker<ApprenticeshipModel>()
                .RuleFor(x => x.Id, f => f.Random.Long(1, 99999))
                .RuleFor(x => x.AccountId, _ => employerAccountId)
                .RuleFor(x => x.AgreedOnDate, f => f.Date.Past(2))
                .RuleFor(x => x.Uln, _ => testSession.Learner.Uln)
                .RuleFor(x => x.Ukprn, _ => testSession.Provider.Ukprn)
                .RuleFor(x => x.EstimatedStartDate, f => f.Date.Past(1))
                .RuleFor(x => x.EstimatedEndDate, (f, x) => x.EstimatedStartDate.AddYears(1))
                .RuleFor(x => x.StandardCode, f => f.Random.Long(1, 999))
                .RuleFor(x => x.ProgrammeType, f => f.Random.Int(1, 99))
                .RuleFor(x => x.FrameworkCode, f => f.Random.Int(1, 999))
                .RuleFor(x => x.PathwayCode, f => f.Random.Int(1, 99))
                .RuleFor(x => x.LegalEntityName, f => f.Company.CompanyName())
                .RuleFor(x => x.TransferSendingEmployerAccountId, _ => null)
                .RuleFor(x => x.StopDate, _ => null)
                .RuleFor(x => x.Priority, f => f.Random.Int(1, 10))
                .RuleFor(x => x.Status, _ => ApprenticeshipStatus.Active)
                .RuleFor(x => x.IsLevyPayer, f => f.Random.Bool())
                .RuleFor(x => x.ApprenticeshipEmployerType, employerType)
                .RuleFor(x => x.CreationDate, _ => DateTimeOffset.UtcNow)
                .RuleFor(x => x.CourseType, _ => CourseType.Apprenticeship)
                .RuleFor(x => x.LearningType, _ => LearningType.Apprenticeship)
                .RuleFor(x => x.CourseCode, f_ => "ZPROG001")
                .RuleFor(x => x.ApprenticeshipPriceEpisodes, _ => new List<ApprenticeshipPriceEpisodeModel>())
                .RuleFor(x => x.ApprenticeshipPauses, _ => new List<ApprenticeshipPauseModel>());

            var apprenticeship = apprenticeshipModelFaker.Generate(1).First();

            testSession.DataContext.Apprenticeships.Add(apprenticeship);
            await testSession.DataContext.SaveChangesAsync();
        }

        private async Task GenerateFundingSourceLevyTransaction()
        {
            var levyAmountFaker = new Faker<CalculatedRequiredLevyAmount>()
                .RuleFor(x => x.AccountId, _ => employerAccountId)
                .RuleFor(x => x.TransactionType, _ => TransactionType.Learning)
                .RuleFor(x => x.ContractType, _ => ContractType.Act1)
                .RuleFor(x => x.EventId, _ => Guid.NewGuid())
                .RuleFor(x => x.EarningEventId, _ => Guid.NewGuid())
                .RuleFor(x => x.ClawbackSourcePaymentEventId, _ => null)
                .RuleFor(x => x.JobId, testSession.JobId)
                .RuleFor(x => x.Ukprn, testSession.Learner.Ukprn)
                .RuleFor(x => x.CollectionPeriod, collectionPeriod)
                .RuleFor(x => x.AmountDue, paymentAmount)
                .RuleFor(x => x.PriceEpisodeIdentifier, f => $"{f.Random.Number(100000000, 999999999)}-" + $"{f.Date.Recent(365):yyyy-MM-dd}")
                .RuleFor(x => x.IlrSubmissionDateTime, f => f.Date.Recent(10))
                .RuleFor(x => x.IlrFileName, f => f.Random.AlphaNumeric(10))
                .RuleFor(x => x.EventTime, f => f.Date.RecentOffset(10))
                .RuleFor(x => x.StartDate, f => f.Date.Past(3).Date)
                .RuleFor(x => x.PlannedEndDate, (f, x) => x.StartDate.AddMonths(f.Random.Int(12, 48)))
                .RuleFor(x => x.ActualEndDate, _ => null)
                .RuleFor(x => x.CompletionStatus, _ => (byte)0)
                .RuleFor(x => x.CompletionAmount, _ => 0m)
                .RuleFor(x => x.InstalmentAmount, f => f.Random.Decimal(100m, 500m))
                .RuleFor(x => x.NumberOfInstalments, f => f.Random.Short(1, 36))
                .RuleFor(x => x.LearningStartDate, (_, x) => x.StartDate)
                .RuleFor(x => x.ApprenticeshipId, f => f.Random.Long(1, 1_000_000))
                .RuleFor(x => x.ApprenticeshipPriceEpisodeId, f => f.Random.Long(1, 1_000_000))
                .RuleFor(x => x.ApprenticeshipEmployerType, employerType)
                .RuleFor(x => x.OnProgrammeEarningType, f => f.PickRandom<OnProgrammeEarningType>())
                .RuleFor(x => x.SfaContributionPercentage, _ => sfaContributionPercentage)
                .RuleFor(x => x.AgeAtStartOfLearning, f => f.Random.Int(16, 65))
                .RuleFor(x => x.CourseType, _ => CourseType.Apprenticeship)
                .RuleFor(x => x.Priority, f => f.Random.Int(1, 10))
                .RuleFor(x => x.AgreementId, f => $"AGR-{f.Random.AlphaNumeric(12)}")
                .RuleFor(x => x.AgreedOnDate, f =>  f.Date.Past(2).Date)
                .RuleFor(x => x.ReportingAimFundingLineType, _ => "funding line type")
                .RuleFor(x => x.FundingPlatformType, _ => FundingPlatformType.SubmitLearnerData);

            calculatedRequiredLevyAmount = levyAmountFaker.Generate(1).First();

            var learnerFaker = new Faker<Learner>()
                .RuleFor(x => x.Uln, _ => testSession.Learner.Uln)
                .RuleFor(x => x.ReferenceNumber, f => f.Random.AlphaNumeric(5));

            calculatedRequiredLevyAmount.Learner = learnerFaker.Generate(1).First();

            var learningAimFaker = new Faker<LearningAim>()
                .RuleFor(x => x.StartDate, _ => calculatedRequiredLevyAmount.StartDate)
                .RuleFor(x => x.LearningType, _ => LearningType.Apprenticeship)
                .RuleFor(x => x.Reference, _ => calculatedRequiredLevyAmount.Learner.ReferenceNumber)
                .RuleFor(x => x.FundingLineType, _ => "funding line type");

            calculatedRequiredLevyAmount.LearningAim = learningAimFaker.Generate(1).First();

            var calculatedRequiredLevyAmountJson = JsonSerializer.Serialize(calculatedRequiredLevyAmount);

            var levyTransactionFaker = new Faker<LevyTransactionModel>()
           .RuleFor(x => x.Ukprn, _ => testSession.Learner.Ukprn)
           .RuleFor(x => x.CollectionPeriod, _ => collectionPeriod.Period)
           .RuleFor(x => x.AcademicYear, _ => collectionPeriod.AcademicYear)
           .RuleFor(x => x.DeliveryPeriod, _ => collectionPeriod.Period)
           .RuleFor(x => x.JobId, _ => testSession.JobId)
           .RuleFor(x => x.AccountId, _ => employerAccountId)
           .RuleFor(x => x.TransferSenderAccountId, _ => null)
           .RuleFor(x => x.RequiredPaymentEventId, _ => Guid.NewGuid())
           .RuleFor(x => x.EarningEventId, _ => Guid.NewGuid())
           .RuleFor(x => x.Amount, _ => paymentAmount)
           .RuleFor(x => x.MessagePayload, _ => calculatedRequiredLevyAmountJson)
           .RuleFor(x => x.MessageType, _ => "SFA.DAS.Payments.RequiredPayments.Messages.Events.CalculatedRequiredLevyAmount")
           .RuleFor(x => x.IlrSubmissionDateTime, _ => calculatedRequiredLevyAmount.IlrSubmissionDateTime)
           .RuleFor(x => x.FundingAccountId, _ => employerAccountId)
           .RuleFor(x => x.TransactionType, _ => TransactionType.Learning)
           .RuleFor(x => x.SfaContributionPercentage, _ => sfaContributionPercentage)
           .RuleFor(x => x.LearnerUln, _ => testSession.Learner.Uln)
           .RuleFor(x => x.LearnerReferenceNumber, f => f.Random.AlphaNumeric(10))
           .RuleFor(x => x.LearningAimReference, f => $"ZPROG{f.Random.Number(100, 999)}")
           .RuleFor(x => x.LearningAimProgrammeType, f => f.Random.Int(2, 25))
           .RuleFor(x => x.LearningAimStandardCode,f => f.Random.Int(1, 999))
           .RuleFor(x => x.LearningAimFrameworkCode, f=> f.Random.Int(1, 999))
           .RuleFor(x => x.LearningAimPathwayCode, f =>  f.Random.Int(1, 99))
           .RuleFor(x => x.LearningAimFundingLineType, _ => "funding line type")
           .RuleFor(x => x.LearningStartDate, _ => calculatedRequiredLevyAmount.LearningStartDate)
           .RuleFor(x => x.ApprenticeshipId, _ => calculatedRequiredLevyAmount.ApprenticeshipId)
           .RuleFor(x => x.ApprenticeshipEmployerType, _ => employerType)
           .RuleFor(x => x.ClawbackSourcePaymentEventId, _ => null)
           .RuleFor(x => x.FundingPlatformType, _ => FundingPlatformType.SubmitLearnerData)
           .RuleFor(x => x.CourseType, _ => CourseType.Apprenticeship)
           .RuleFor(x => x.LearningType, _ => LearningType.Apprenticeship)
           .RuleFor(x => x.CourseCode,f => $"ST{f.Random.Number(100, 999)}");

            var levyTransactionModel = levyTransactionFaker.Generate(1).First();

            testSession.DataContext.LevyTransactions.Add(levyTransactionModel);
            await testSession.DataContext.SaveChangesAsync();
        }

        private async Task GenerateRequiredPaymentEvent()
        {
            var requiredPaymentFaker = new Faker<RequiredPaymentEventModel>()
            .RuleFor(x => x.EventId, _ => Guid.NewGuid())
            .RuleFor(x => x.EarningEventId, _ => Guid.NewGuid())
            .RuleFor(x => x.ClawbackSourcePaymentEventId, _ => null)
            .RuleFor(x => x.PriceEpisodeIdentifier,
                f => $"{f.Random.Number(100000000, 999999999)}-{f.Date.Recent():yyyy-MM-dd}")
            .RuleFor(x => x.Ukprn, _ => testSession.Learner.Ukprn)
            .RuleFor(x => x.ContractType, _ => ContractType.Act1)
            .RuleFor(x => x.TransactionType, _ => TransactionType.Learning)
            .RuleFor(x => x.SfaContributionPercentage, _ => sfaContributionPercentage)
            .RuleFor(x => x.Amount, _ => paymentAmount)
            .RuleFor(x => x.CollectionPeriod, _ => collectionPeriod)
            .RuleFor(x => x.DeliveryPeriod, _ => collectionPeriod.Period)
            .RuleFor(x => x.LearnerReferenceNumber, f => f.Random.AlphaNumeric(12))
            .RuleFor(x => x.LearnerUln, _ => testSession.Learner.Uln)
            .RuleFor(x => x.LearningAimReference, f => $"ZPROG{f.Random.Number(100, 999)}")
            .RuleFor(x => x.LearningAimProgrammeType, f => f.Random.Int(2, 25))
            .RuleFor(x => x.LearningAimStandardCode, f => f.Random.Int(1, 999))
            .RuleFor(x => x.LearningAimFrameworkCode, f => f.Random.Int(1, 999))
            .RuleFor(x => x.LearningAimPathwayCode, f =>  f.Random.Int(1, 99))
            .RuleFor(x => x.LearningAimFundingLineType, _ => "funding line type")
            .RuleFor(x => x.AgreementId, f => $"AGR-{f.Random.AlphaNumeric(10)}")
            .RuleFor(x => x.IlrSubmissionDateTime, _ => calculatedRequiredLevyAmount.IlrSubmissionDateTime)
            .RuleFor(x => x.JobId, _ => testSession.JobId)
            .RuleFor(x => x.EventTime, f => f.Date.RecentOffset(30))
            .RuleFor(x => x.AccountId, _ => employerAccountId)
            .RuleFor(x => x.TransferSenderAccountId, _ => null)
            .RuleFor(x => x.StartDate, _ => calculatedRequiredLevyAmount.StartDate)
            .RuleFor(x => x.PlannedEndDate, _ => calculatedRequiredLevyAmount.PlannedEndDate)
            .RuleFor(x => x.ActualEndDate, _ => calculatedRequiredLevyAmount.ActualEndDate)
            .RuleFor(x => x.CompletionStatus, _ => calculatedRequiredLevyAmount.CompletionStatus)
            .RuleFor(x => x.CompletionAmount, _ => calculatedRequiredLevyAmount.CompletionAmount)
            .RuleFor(x => x.InstalmentAmount, _ => calculatedRequiredLevyAmount.InstalmentAmount)
            .RuleFor(x => x.NumberOfInstalments, _ => calculatedRequiredLevyAmount.NumberOfInstalments)
            .RuleFor(x => x.LearningStartDate, _ => calculatedRequiredLevyAmount.LearningStartDate)
            .RuleFor(x => x.ApprenticeshipId, _ => calculatedRequiredLevyAmount.ApprenticeshipId)
            .RuleFor(x => x.ApprenticeshipPriceEpisodeId, _ => calculatedRequiredLevyAmount.ApprenticeshipPriceEpisodeId)
            .RuleFor(x => x.ApprenticeshipEmployerType, _ => employerType)
            .RuleFor(x => x.NonPaymentReason, _ => null)
            .RuleFor(x => x.EventType, _ => "RequiredPaymentEvent")
            .RuleFor(x => x.AgeAtStartOfLearning, f => f.Random.Int(16, 65))
            .RuleFor(x => x.CourseType, _ => CourseType.Apprenticeship)
            .RuleFor(x => x.LearningType, _ => LearningType.Apprenticeship)
            .RuleFor(x => x.CourseCode, f => $"ST{f.Random.Number(100, 999)}");

            var requiredPaymentEventModel = requiredPaymentFaker.Generate(1).First();

            testSession.DataContext.RequiredPaymentEvents.Add(requiredPaymentEventModel);
            await testSession.DataContext.SaveChangesAsync();
        }

    }
}
