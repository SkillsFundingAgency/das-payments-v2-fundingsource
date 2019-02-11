﻿using System;
using System.Collections.Generic;
using FluentAssertions;
using NUnit.Framework;
using SFA.DAS.Payments.FundingSource.Domain.Interface;
using SFA.DAS.Payments.FundingSource.Domain.Services;

namespace SFA.DAS.Payments.FundingSource.Domain.UnitTests.GivenASortableKeyGenerator
{
    [TestFixture]
    public class WhenCallingGenerate
    {
        protected static IGenerateSortableKeys sut;

        [SetUp]
        public void Setup()
        {
            sut = new SortableKeyGenerator();
        }
        
        [Test]
        public void ThenTheResultsShouldSortRefundsBeforePayments()
        {
            var keys = new List<string>();
            var paymentKey = sut.Generate(1, 0, DateTime.MaxValue, 0);
            keys.Add(paymentKey);
            var refundKey = sut.Generate(-100, 0, DateTime.MaxValue, 0);
            keys.Add(refundKey);

            keys.Sort();

            keys[0].Should().Be(refundKey);
            keys[1].Should().Be(paymentKey);
        }

        [Test]
        public void ThenTheResultsShouldSortByPriority()
        {
            var keys = new List<string>();
            var expectedFirst = sut.Generate(1, 1, DateTime.MaxValue, 0);
            keys.Add(expectedFirst);
            var expectedSecond = sut.Generate(1, 2, DateTime.MaxValue, 0);
            keys.Add(expectedSecond);

            keys.Sort();

            keys[0].Should().Be(expectedFirst);
            keys[1].Should().Be(expectedSecond);
        }

        [Test]
        public void ThenTheResultsShouldSortByDateAgreed()
        {
            var keys = new List<string>();
            var expectedFirst = sut.Generate(1, 1, DateTime.MinValue, 0);
            keys.Add(expectedFirst);
            var expectedSecond = sut.Generate(1, 1, DateTime.MaxValue, 0);
            keys.Add(expectedSecond);

            keys.Sort();

            keys[0].Should().Be(expectedFirst);
            keys[1].Should().Be(expectedSecond);
        }

        [Test]
        public void ThenTheResultsShouldSortByUln()
        {
            var keys = new List<string>();
            var expectedFirst = sut.Generate(1, 1, DateTime.MaxValue, 10);
            keys.Add(expectedFirst);
            var expectedSecond = sut.Generate(1, 1, DateTime.MaxValue, 100);
            keys.Add(expectedSecond);

            keys.Sort();

            keys[0].Should().Be(expectedFirst);
            keys[1].Should().Be(expectedSecond);
        }

        [TestFixture]
        public class AndAllEventsArePayments 
        {
            [Test]
            public void ThenTheResultsShouldSortByPriority()
            {
                var keys = new List<string>();
                var lowPriorityKey = sut.Generate(100, 2, DateTime.MaxValue, 0);
                keys.Add(lowPriorityKey);
                var highPriorityKey = sut.Generate(1, 1, DateTime.MaxValue, 0);
                keys.Add(highPriorityKey);

                keys.Sort();

                keys[0].Should().Be(highPriorityKey);
                keys[1].Should().Be(lowPriorityKey);
            }
        }

        [TestFixture]
        public class AndAllEventsAreRefunds 
        {
            [Test]
            public void ThenTheResultsShouldSortByPriority()
            {
                var keys = new List<string>();
                var lowPriorityKey = sut.Generate(-1, 2, DateTime.MaxValue, 0);
                keys.Add(lowPriorityKey);
                var highPriorityKey = sut.Generate(-100, 1, DateTime.MaxValue, 0);
                keys.Add(highPriorityKey);

                keys.Sort();

                keys[0].Should().Be(highPriorityKey);
                keys[1].Should().Be(lowPriorityKey);
            }
        }
    }
}
