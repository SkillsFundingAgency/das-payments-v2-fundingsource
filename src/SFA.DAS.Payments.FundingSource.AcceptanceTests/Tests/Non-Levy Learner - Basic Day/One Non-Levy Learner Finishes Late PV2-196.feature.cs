﻿// ------------------------------------------------------------------------------
//  <auto-generated>
//      This code was generated by SpecFlow (http://www.specflow.org/).
//      SpecFlow Version:2.4.0.0
//      SpecFlow Generator Version:2.4.0.0
// 
//      Changes to this file may cause incorrect behavior and will be lost if
//      the code is regenerated.
//  </auto-generated>
// ------------------------------------------------------------------------------
#region Designer generated code
#pragma warning disable
namespace SFA.DAS.Payments.FundingSource.AcceptanceTests.Tests.Non_LevyLearner_BasicDay
{
    using TechTalk.SpecFlow;
    
    
    [System.CodeDom.Compiler.GeneratedCodeAttribute("TechTalk.SpecFlow", "2.4.0.0")]
    [System.Runtime.CompilerServices.CompilerGeneratedAttribute()]
    [NUnit.Framework.TestFixtureAttribute()]
    [NUnit.Framework.DescriptionAttribute("One Non-Levy Learner Finishes Late PV2-196")]
    public partial class OneNon_LevyLearnerFinishesLatePV2_196Feature
    {
        
        private TechTalk.SpecFlow.ITestRunner testRunner;
        
#line 1 "One Non-Levy Learner Finishes Late PV2-196.feature"
#line hidden
        
        [NUnit.Framework.OneTimeSetUpAttribute()]
        public virtual void FeatureSetup()
        {
            testRunner = TechTalk.SpecFlow.TestRunnerManager.GetTestRunner();
            TechTalk.SpecFlow.FeatureInfo featureInfo = new TechTalk.SpecFlow.FeatureInfo(new System.Globalization.CultureInfo("en-US"), "One Non-Levy Learner Finishes Late PV2-196", "Provider earnings and payments where learner completes later than planned", ProgrammingLanguage.CSharp, ((string[])(null)));
            testRunner.OnFeatureStart(featureInfo);
        }
        
        [NUnit.Framework.OneTimeTearDownAttribute()]
        public virtual void FeatureTearDown()
        {
            testRunner.OnFeatureEnd();
            testRunner = null;
        }
        
        [NUnit.Framework.SetUpAttribute()]
        public virtual void TestInitialize()
        {
        }
        
        [NUnit.Framework.TearDownAttribute()]
        public virtual void ScenarioTearDown()
        {
            testRunner.OnScenarioEnd();
        }
        
        public virtual void ScenarioInitialize(TechTalk.SpecFlow.ScenarioInfo scenarioInfo)
        {
            testRunner.OnScenarioInitialize(scenarioInfo);
            testRunner.ScenarioContext.ScenarioContainer.RegisterInstanceAs<NUnit.Framework.TestContext>(NUnit.Framework.TestContext.CurrentContext);
        }
        
        public virtual void ScenarioStart()
        {
            testRunner.OnScenarioStart();
        }
        
        public virtual void ScenarioCleanup()
        {
            testRunner.CollectScenarioErrors();
        }
        
        public virtual void FeatureBackground()
        {
#line 4
#line 5
 testRunner.Given("a learner is undertaking a training with a training provider", ((string)(null)), ((TechTalk.SpecFlow.Table)(null)), "Given ");
#line 6
 testRunner.And("the SFA contribution percentage is 90%", ((string)(null)), ((TechTalk.SpecFlow.Table)(null)), "And ");
#line 7
 testRunner.And("the payments are for the current collection year", ((string)(null)), ((TechTalk.SpecFlow.Table)(null)), "And ");
#line hidden
        }
        
        [NUnit.Framework.TestAttribute()]
        [NUnit.Framework.DescriptionAttribute("A non-DAS learner, learner finishes late")]
        [NUnit.Framework.CategoryAttribute("NonDas_BasicDay")]
        [NUnit.Framework.CategoryAttribute("finishes_late")]
        [NUnit.Framework.CategoryAttribute("Completion")]
        [NUnit.Framework.CategoryAttribute("(TT2)")]
        [NUnit.Framework.CategoryAttribute("CoInvested")]
        public virtual void ANon_DASLearnerLearnerFinishesLate()
        {
            TechTalk.SpecFlow.ScenarioInfo scenarioInfo = new TechTalk.SpecFlow.ScenarioInfo("A non-DAS learner, learner finishes late", null, new string[] {
                        "NonDas_BasicDay",
                        "finishes_late",
                        "Completion",
                        "(TT2)",
                        "CoInvested"});
#line 15
this.ScenarioInitialize(scenarioInfo);
            this.ScenarioStart();
#line 4
this.FeatureBackground();
#line 16
 testRunner.Given("the current collection period is R05", ((string)(null)), ((TechTalk.SpecFlow.Table)(null)), "Given ");
#line hidden
            TechTalk.SpecFlow.Table table1 = new TechTalk.SpecFlow.Table(new string[] {
                        "PriceEpisodeIdentifier",
                        "Delivery Period",
                        "TransactionType",
                        "Amount"});
            table1.AddRow(new string[] {
                        "p2",
                        "5",
                        "Completion (TT2)",
                        "3000"});
#line 18
 testRunner.And("the required payments component generates the following contract type 2 payable e" +
                    "arnings:", ((string)(null)), table1, "And ");
#line 22
 testRunner.When("required payments event is received", ((string)(null)), ((TechTalk.SpecFlow.Table)(null)), "When ");
#line hidden
            TechTalk.SpecFlow.Table table2 = new TechTalk.SpecFlow.Table(new string[] {
                        "PriceEpisodeIdentifier",
                        "Delivery Period",
                        "TransactionType",
                        "FundingSource",
                        "Amount"});
            table2.AddRow(new string[] {
                        "p2",
                        "5",
                        "Completion (TT2)",
                        "CoInvestedSfa (FS2)",
                        "2700"});
            table2.AddRow(new string[] {
                        "p2",
                        "5",
                        "Completion (TT2)",
                        "CoInvestedEmploer (FS3)",
                        "300"});
#line 23
 testRunner.Then("the payment source component will generate the following contract type 2 coinvest" +
                    "ed payments:", ((string)(null)), table2, "Then ");
#line hidden
            this.ScenarioCleanup();
        }
        
        [NUnit.Framework.TestAttribute()]
        [NUnit.Framework.DescriptionAttribute("A non-DAS learner, learner finishes late - no history")]
        [NUnit.Framework.CategoryAttribute("NonDas_BasicDay")]
        [NUnit.Framework.CategoryAttribute("finishes_late")]
        [NUnit.Framework.CategoryAttribute("Learning")]
        [NUnit.Framework.CategoryAttribute("(TT1)")]
        [NUnit.Framework.CategoryAttribute("Completion")]
        [NUnit.Framework.CategoryAttribute("(TT2)")]
        [NUnit.Framework.CategoryAttribute("CoInvested")]
        public virtual void ANon_DASLearnerLearnerFinishesLate_NoHistory()
        {
            TechTalk.SpecFlow.ScenarioInfo scenarioInfo = new TechTalk.SpecFlow.ScenarioInfo("A non-DAS learner, learner finishes late - no history", null, new string[] {
                        "NonDas_BasicDay",
                        "finishes_late",
                        "Learning",
                        "(TT1)",
                        "Completion",
                        "(TT2)",
                        "CoInvested"});
#line 35
this.ScenarioInitialize(scenarioInfo);
            this.ScenarioStart();
#line 4
this.FeatureBackground();
#line 36
 testRunner.Given("the current collection period is R05", ((string)(null)), ((TechTalk.SpecFlow.Table)(null)), "Given ");
#line hidden
            TechTalk.SpecFlow.Table table3 = new TechTalk.SpecFlow.Table(new string[] {
                        "PriceEpisodeIdentifier",
                        "Delivery Period",
                        "TransactionType",
                        "Amount"});
            table3.AddRow(new string[] {
                        "p2",
                        "1",
                        "Learning (TT1)",
                        "1000"});
            table3.AddRow(new string[] {
                        "p2",
                        "5",
                        "Completion (TT2)",
                        "3000"});
#line 38
 testRunner.And("the required payments component generates the following contract type 2 payable e" +
                    "arnings:", ((string)(null)), table3, "And ");
#line 43
 testRunner.When("required payments event is received", ((string)(null)), ((TechTalk.SpecFlow.Table)(null)), "When ");
#line hidden
            TechTalk.SpecFlow.Table table4 = new TechTalk.SpecFlow.Table(new string[] {
                        "PriceEpisodeIdentifier",
                        "Delivery Period",
                        "TransactionType",
                        "FundingSource",
                        "Amount"});
            table4.AddRow(new string[] {
                        "p2",
                        "1",
                        "Learning (TT1)",
                        "CoInvestedSfa (FS2)",
                        "900"});
            table4.AddRow(new string[] {
                        "p2",
                        "1",
                        "Learning (TT1)",
                        "CoInvestedEmploer (FS3)",
                        "100"});
            table4.AddRow(new string[] {
                        "p2",
                        "5",
                        "Completion (TT2)",
                        "CoInvestedSfa (FS2)",
                        "2700"});
            table4.AddRow(new string[] {
                        "p2",
                        "5",
                        "Completion (TT2)",
                        "CoInvestedEmploer (FS3)",
                        "300"});
#line 44
 testRunner.Then("the payment source component will generate the following contract type 2 coinvest" +
                    "ed payments:", ((string)(null)), table4, "Then ");
#line hidden
            this.ScenarioCleanup();
        }
    }
}
#pragma warning restore
#endregion