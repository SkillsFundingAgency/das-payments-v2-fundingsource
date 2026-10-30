using Bogus;
using NUnit.Framework;
using SFA.DAS.Payments.Model.Core;
using SFA.DAS.Payments.Model.Core.Audit;
using SFA.DAS.Payments.Model.Core.Entities;
using SFA.DAS.Payments.Model.Core.OnProgramme;
using SFA.DAS.Payments.PeriodEnd.Messages.Events;
using SFA.DAS.Payments.RequiredPayments.Messages.Events;

namespace SFA.DAS.Payments.FundingSource.Tests.Specs.StepDefinitions
{
    [Binding]
    public class ShortCourseFundingSourceSteps
    {
        private const string CourseCode = "ZSC00001";
        private const decimal RequiredAmount = 300m;

        private readonly MessagingContext messagingContext;
        private readonly TestSession testSession;
        private readonly CollectionPeriod collectionPeriod = new CollectionPeriodBuilder().WithDate(DateTime.Today).Build();
        private readonly long employerAccountId = new Randomizer().Long(1, 100000);
        private string fundingLineType = string.Empty;
        private ApprenticeshipEmployerType employerType;
        private Guid requiredPaymentEventId;
        private decimal expectedAmount;
        private int expectedPaymentCount;

        public ShortCourseFundingSourceSteps(MessagingContext messagingContext, TestSession testSession)
        {
            this.messagingContext = messagingContext;
            this.testSession = testSession;
        }

        [Given("the learning is an Apprenticeship Unit short course funded under the {string} funding line")]
        public void GivenTheLearningIsAnApprenticeshipUnitShortCourseFundedUnderTheFundingLine(string fundingLine)
        {
            fundingLineType = fundingLine;
        }

        [Given("the employer is Non-Levy funded")]
        public void GivenTheEmployerIsNonLevyFunded()
        {
            employerType = ApprenticeshipEmployerType.NonLevy;
        }

        [Given("the employer is Levy funded with sufficient funds in their account")]
        public async Task GivenTheEmployerIsLevyFundedWithSufficientFundsInTheirAccount()
        {
            employerType = ApprenticeshipEmployerType.Levy;
            testSession.DataContext.LevyAccounts.Add(new LevyAccountModel
            {
                AccountId = employerAccountId,
                Balance = 10000m,
                IsLevyPayer = true,
                AccountName = "Automated Test Account",
                TransferAllowance = 0m
            });
            testSession.DataContext.Apprenticeships.Add(new ApprenticeshipModel
            {
                Id = new Randomizer().Long(100000, 99999999),
                AccountId = employerAccountId,
                AgreedOnDate = DateTime.Today.AddYears(-1),
                Uln = testSession.Learner.Uln,
                Ukprn = testSession.Provider.Ukprn,
                EstimatedStartDate = DateTime.Today.AddMonths(-1),
                EstimatedEndDate = DateTime.Today.AddMonths(11),
                StandardCode = 0,
                ProgrammeType = 0,
                FrameworkCode = 0,
                PathwayCode = 0,
                LegalEntityName = "Automated Test Employer",
                TransferSendingEmployerAccountId = null,
                StopDate = null,
                Priority = 1,
                Status = ApprenticeshipStatus.Active,
                IsLevyPayer = true,
                ApprenticeshipEmployerType = ApprenticeshipEmployerType.Levy,
                CreationDate = DateTimeOffset.UtcNow,
                CourseType = CourseType.ShortCourse,
                LearningType = LearningType.ApprenticeshipUnit,
                CourseCode = CourseCode,
                ApprenticeshipPriceEpisodes = new List<ApprenticeshipPriceEpisodeModel>(),
                ApprenticeshipPauses = new List<ApprenticeshipPauseModel>()
            });
            await testSession.DataContext.SaveChangesAsync();
        }

        [When("a co-invested payment required payment is received for the short course")]
        public async Task WhenACoInvestedPaymentRequiredPaymentIsReceivedForTheShortCourse()
        {
            await SendCoInvestedRequiredPayment(RequiredAmount, null);
        }

        [When("a co-invested clawback required payment is received for the short course")]
        public async Task WhenACoInvestedClawbackRequiredPaymentIsReceivedForTheShortCourse()
        {
            await SendCoInvestedRequiredPayment(-RequiredAmount, Guid.NewGuid());
        }

        [When("a levy payment required payment is received for the short course")]
        public async Task WhenALevyPaymentRequiredPaymentIsReceivedForTheShortCourse()
        {
            await SendLevyRequiredPayment(RequiredAmount, null);
        }

        [When("a levy clawback required payment is received for the short course")]
        public async Task WhenALevyClawbackRequiredPaymentIsReceivedForTheShortCourse()
        {
            await SendLevyRequiredPayment(-RequiredAmount, Guid.NewGuid());
        }

        [When("the period end runs")]
        public async Task WhenThePeriodEndRuns()
        {
            await testSession.WaitForIt(() => testSession.DataContext.LevyTransactions.Any(x =>
                    x.LearnerUln == testSession.Learner.Uln && x.RequiredPaymentEventId == requiredPaymentEventId),
                "The levy transaction was not stored.");

            await messagingContext.Send(new PeriodEndRunningEvent
            {
                CollectionPeriod = collectionPeriod,
                EventId = Guid.NewGuid(),
                EventTime = DateTimeOffset.Now,
                JobId = testSession.JobId
            });
        }

        [Then("the recorded payments carry the funding line type and the short course details")]
        public async Task ThenTheRecordedPaymentsCarryTheFundingLineTypeAndTheShortCourseDetails()
        {
            await testSession.WaitForIt(() => GetPayments().Count == expectedPaymentCount,
                $"Failed to find the {expectedPaymentCount} expected payment(s).");

            foreach (var payment in GetPayments())
            {
                var source = payment.FundingSource;
                Assert.That(payment.ReportingAimFundingLineType, Is.EqualTo(fundingLineType), $"{source} ReportingAimFundingLineType");
                Assert.That(payment.ReportingAimFundingLineType, Does.Not.StartWith("None"), $"{source} ReportingAimFundingLineType");
                Assert.That(payment.CourseType, Is.EqualTo(CourseType.ShortCourse), $"{source} CourseType");
                Assert.That(payment.LearningType, Is.EqualTo(LearningType.ApprenticeshipUnit), $"{source} LearningType");
                Assert.That(payment.CourseCode, Is.EqualTo(CourseCode), $"{source} CourseCode");
            }
        }

        private List<PaymentModel> GetPayments()
        {
            var payments = testSession.DataContext.Payment
                .Where(x => x.Ukprn == testSession.Provider.Ukprn && x.LearnerUln == testSession.Learner.Uln);

            payments = expectedAmount < 0 ? payments.Where(x => x.Amount < 0) : payments.Where(x => x.Amount > 0);

            return payments.ToList();
        }

        private async Task SendCoInvestedRequiredPayment(decimal amount, Guid? clawbackSourcePaymentEventId)
        {
            var requiredPayment = new CalculatedRequiredCoInvestedAmount
            {
                FundingPlatformType = FundingPlatformType.DigitalApprenticeshipService
            };
            Populate(requiredPayment, amount, clawbackSourcePaymentEventId);
            expectedPaymentCount = 2;

            Console.WriteLine($"Sending the co-invested required payment.  Event: {requiredPayment.EventId}, Amount: {amount}");
            await messagingContext.SendToNonLevy(requiredPayment);
        }

        private async Task SendLevyRequiredPayment(decimal amount, Guid? clawbackSourcePaymentEventId)
        {
            var requiredPayment = new CalculatedRequiredLevyAmount
            {
                AgreementId = $"AGR-{Guid.NewGuid():N}".Substring(0, 16),
                AgreedOnDate = DateTime.Today.AddYears(-1),
                Priority = 1,
                FundingPlatformType = FundingPlatformType.DigitalApprenticeshipService
            };
            Populate(requiredPayment, amount, clawbackSourcePaymentEventId);
            expectedPaymentCount = 1;

            Console.WriteLine($"Sending the levy required payment.  Event: {requiredPayment.EventId}, Amount: {amount}");
            await messagingContext.SendToLevyTransaction(requiredPayment);
        }

        private void Populate(CalculatedRequiredOnProgrammeAmount requiredPayment, decimal amount, Guid? clawbackSourcePaymentEventId)
        {
            expectedAmount = amount;
            requiredPaymentEventId = Guid.NewGuid();

            requiredPayment.EventId = requiredPaymentEventId;
            requiredPayment.EarningEventId = Guid.NewGuid();
            requiredPayment.ClawbackSourcePaymentEventId = clawbackSourcePaymentEventId;
            requiredPayment.JobId = testSession.JobId;
            requiredPayment.EventTime = DateTimeOffset.UtcNow;
            requiredPayment.Ukprn = testSession.Provider.Ukprn;
            requiredPayment.Learner = new Learner
            {
                Uln = testSession.Learner.Uln,
                ReferenceNumber = testSession.Learner.LearnRefNumber
            };
            requiredPayment.CollectionPeriod = collectionPeriod;
            requiredPayment.DeliveryPeriod = 1;
            requiredPayment.AccountId = employerAccountId;
            requiredPayment.AmountDue = amount;
            requiredPayment.ContractType = ContractType.Act1;
            requiredPayment.ApprenticeshipEmployerType = employerType;
            requiredPayment.OnProgrammeEarningType = OnProgrammeEarningType.Milestone1;
            requiredPayment.SfaContributionPercentage = 0.95m;
            requiredPayment.CourseType = CourseType.ShortCourse;
            requiredPayment.PriceEpisodeIdentifier = $"{DateTime.Today.AddMonths(-1):yyyy-MM-dd}";
            requiredPayment.IlrSubmissionDateTime = DateTime.UtcNow;
            requiredPayment.IlrFileName = "ILR-TEST";
            requiredPayment.ApprenticeshipId = testSession.GenerateId();
            requiredPayment.ApprenticeshipPriceEpisodeId = testSession.GenerateId();
            requiredPayment.AgeAtStartOfLearning = 21;
            requiredPayment.InstalmentAmount = RequiredAmount;
            requiredPayment.NumberOfInstalments = 1;
            requiredPayment.CompletionStatus = 0;
            requiredPayment.CompletionAmount = 0m;
            requiredPayment.StartDate = DateTime.Today.AddMonths(-1);
            requiredPayment.LearningStartDate = DateTime.Today.AddMonths(-1);
            requiredPayment.PlannedEndDate = DateTime.Today.AddMonths(1);
            requiredPayment.LearningAim = new LearningAim
            {
                Reference = CourseCode,
                CourseCode = CourseCode,
                LearningType = LearningType.ApprenticeshipUnit,
                FundingLineType = fundingLineType,
                StartDate = DateTime.Today.AddMonths(-1)
            };
        }
    }
}
