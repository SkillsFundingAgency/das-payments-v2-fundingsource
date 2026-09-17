namespace SFA.DAS.Payments.FundingSource.Tests.Specs.Models
{

    public class Provider
    {
        public int Ukprn { get; private set; }

        public DateTime LastUsed { get; private set; }

        internal void Use() => LastUsed = DateTime.UtcNow;
    }
}
