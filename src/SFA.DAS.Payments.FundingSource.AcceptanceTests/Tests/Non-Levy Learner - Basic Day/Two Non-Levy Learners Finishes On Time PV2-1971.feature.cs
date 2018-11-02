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
    [NUnit.Framework.DescriptionAttribute("Basic Day")]
    public partial class BasicDayFeature
    {
        
        private TechTalk.SpecFlow.ITestRunner testRunner;
        
#line 1 "Two Non-Levy Learners Finishes On Time PV2-197.feature"
#line hidden
        
        [NUnit.Framework.OneTimeSetUpAttribute()]
        public virtual void FeatureSetup()
        {
            testRunner = TechTalk.SpecFlow.TestRunnerManager.GetTestRunner();
            TechTalk.SpecFlow.FeatureInfo featureInfo = new TechTalk.SpecFlow.FeatureInfo(new System.Globalization.CultureInfo("en-US"), "Basic Day", "Non-Levy - 2 learners - Both finishes on time", ProgrammingLanguage.CSharp, ((string[])(null)));
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
 testRunner.Given("the current collection period is R02", ((string)(null)), ((TechTalk.SpecFlow.Table)(null)), "Given ");
#line 6
 testRunner.And("the payments are for the current collection year", ((string)(null)), ((TechTalk.SpecFlow.Table)(null)), "And ");
#line 7
 testRunner.And("the SFA contribution percentage is 90%", ((string)(null)), ((TechTalk.SpecFlow.Table)(null)), "And ");
#line hidden
            TechTalk.SpecFlow.Table table1 = new TechTalk.SpecFlow.Table(new string[] {
                        "LearnerId"});
            table1.AddRow(new string[] {
                        "L1"});
            table1.AddRow(new string[] {
                        "L2"});
#line 8
 testRunner.And("following learners are undertaking training with a training provider", ((string)(null)), table1, "And ");
#line hidden
        }
        
        [NUnit.Framework.TestAttribute()]
        [NUnit.Framework.DescriptionAttribute("Completion for both")]
        [NUnit.Framework.CategoryAttribute("NonLevy_BasicDay")]
        [NUnit.Framework.CategoryAttribute("OnTime")]
        public virtual void CompletionForBoth()
        {
            TechTalk.SpecFlow.ScenarioInfo scenarioInfo = new TechTalk.SpecFlow.ScenarioInfo("Completion for both", null, new string[] {
                        "NonLevy_BasicDay",
                        "OnTime"});
#line 15
this.ScenarioInitialize(scenarioInfo);
            this.ScenarioStart();
#line 4
this.FeatureBackground();
#line hidden
            TechTalk.SpecFlow.Table table2 = new TechTalk.SpecFlow.Table(new string[] {
                        "LearnerId",
                        "PriceEpisodeIdentifier",
                        "Delivery Period",
                        "TransactionType",
                        "Amount"});
            table2.AddRow(new string[] {
                        "L1",
                        "p2",
                        "2",
                        "Completion (TT2)",
                        "3000"});
            table2.AddRow(new string[] {
                        "L2",
                        "p3",
                        "2",
                        "Completion (TT2)",
                        "2400"});
#line 16
 testRunner.Given("the required payments component generates the following contract type 2 payable e" +
                    "arnings:", ((string)(null)), table2, "Given ");
#line 20
 testRunner.When("required payments event is received", ((string)(null)), ((TechTalk.SpecFlow.Table)(null)), "When ");
#line hidden
            TechTalk.SpecFlow.Table table3 = new TechTalk.SpecFlow.Table(new string[] {
                        "LearnerId",
                        "PriceEpisodeIdentifier",
                        "Delivery Period",
                        "TransactionType",
                        "FundingSource",
                        "Amount"});
            table3.AddRow(new string[] {
                        "L1",
                        "p2",
                        "2",
                        "Completion (TT2)",
                        "CoInvestedSfa (FS2)",
                        "2700"});
            table3.AddRow(new string[] {
                        "L1",
                        "p2",
                        "2",
                        "Completion (TT2)",
                        "CoInvestedEmployer (FS3)",
                        "300"});
            table3.AddRow(new string[] {
                        "L2",
                        "p3",
                        "2",
                        "Completion (TT2)",
                        "CoInvestedSfa (FS2)",
                        "2160"});
            table3.AddRow(new string[] {
                        "L2",
                        "p3",
                        "2",
                        "Completion (TT2)",
                        "CoInvestedEmployer (FS3)",
                        "240"});
#line 21
 testRunner.Then("the payment source component will generate the following contract type 2 coinvest" +
                    "ed payments:", ((string)(null)), table3, "Then ");
#line hidden
            this.ScenarioCleanup();
        }
        
        [NUnit.Framework.TestAttribute()]
        [NUnit.Framework.DescriptionAttribute("Learning and Completion for both - No history")]
        [NUnit.Framework.CategoryAttribute("NonLevy_BasicDay")]
        [NUnit.Framework.CategoryAttribute("OnTime")]
        [NUnit.Framework.CategoryAttribute("NoHistory")]
        public virtual void LearningAndCompletionForBoth_NoHistory()
        {
            TechTalk.SpecFlow.ScenarioInfo scenarioInfo = new TechTalk.SpecFlow.ScenarioInfo("Learning and Completion for both - No history", null, new string[] {
                        "NonLevy_BasicDay",
                        "OnTime",
                        "NoHistory"});
#line 31
this.ScenarioInitialize(scenarioInfo);
            this.ScenarioStart();
#line 4
this.FeatureBackground();
#line hidden
            TechTalk.SpecFlow.Table table4 = new TechTalk.SpecFlow.Table(new string[] {
                        "LearnerId",
                        "PriceEpisodeIdentifier",
                        "Delivery Period",
                        "TransactionType",
                        "Amount"});
            table4.AddRow(new string[] {
                        "L1",
                        "p2",
                        "1",
                        "Learning (TT1)",
                        "1000"});
            table4.AddRow(new string[] {
                        "L1",
                        "p2",
                        "2",
                        "Completion (TT2)",
                        "3000"});
            table4.AddRow(new string[] {
                        "L2",
                        "p3",
                        "1",
                        "Learning (TT1)",
                        "800"});
            table4.AddRow(new string[] {
                        "L2",
                        "p3",
                        "2",
                        "Completion (TT2)",
                        "2400"});
#line 32
 testRunner.Given("the required payments component generates the following contract type 2 payable e" +
                    "arnings:", ((string)(null)), table4, "Given ");
#line 38
 testRunner.When("required payments event is received", ((string)(null)), ((TechTalk.SpecFlow.Table)(null)), "When ");
#line hidden
            TechTalk.SpecFlow.Table table5 = new TechTalk.SpecFlow.Table(new string[] {
                        "LearnerId",
                        "PriceEpisodeIdentifier",
                        "Delivery Period",
                        "TransactionType",
                        "FundingSource",
                        "Amount"});
            table5.AddRow(new string[] {
                        "L1",
                        "p2",
                        "1",
                        "Learning (TT1)",
                        "CoInvestedSfa (FS2)",
                        "900"});
            table5.AddRow(new string[] {
                        "L1",
                        "p2",
                        "1",
                        "Learning (TT1)",
                        "CoInvestedEmployer (FS3)",
                        "100"});
            table5.AddRow(new string[] {
                        "L1",
                        "p2",
                        "2",
                        "Completion (TT2)",
                        "CoInvestedSfa (FS2)",
                        "2700"});
            table5.AddRow(new string[] {
                        "L1",
                        "p2",
                        "2",
                        "Completion (TT2)",
                        "CoInvestedEmployer (FS3)",
                        "300"});
            table5.AddRow(new string[] {
                        "L2",
                        "p3",
                        "1",
                        "Learning (TT1)",
                        "CoInvestedSfa (FS2)",
                        "720"});
            table5.AddRow(new string[] {
                        "L2",
                        "p3",
                        "1",
                        "Learning (TT1)",
                        "CoInvestedEmployer (FS3)",
                        "80"});
            table5.AddRow(new string[] {
                        "L2",
                        "p3",
                        "2",
                        "Completion (TT2)",
                        "CoInvestedSfa (FS2)",
                        "2160"});
            table5.AddRow(new string[] {
                        "L2",
                        "p3",
                        "2",
                        "Completion (TT2)",
                        "CoInvestedEmployer (FS3)",
                        "240"});
#line 39
 testRunner.Then("the payment source component will generate the following contract type 2 coinvest" +
                    "ed payments:", ((string)(null)), table5, "Then ");
#line hidden
            this.ScenarioCleanup();
        }
        
        [NUnit.Framework.TestAttribute()]
        [NUnit.Framework.DescriptionAttribute("Learning for 1 and Completion for both - 1 learner has history")]
        [NUnit.Framework.CategoryAttribute("NonLevy_BasicDay")]
        [NUnit.Framework.CategoryAttribute("OnTime")]
        [NUnit.Framework.CategoryAttribute("PartialHistory")]
        public virtual void LearningFor1AndCompletionForBoth_1LearnerHasHistory()
        {
            TechTalk.SpecFlow.ScenarioInfo scenarioInfo = new TechTalk.SpecFlow.ScenarioInfo("Learning for 1 and Completion for both - 1 learner has history", null, new string[] {
                        "NonLevy_BasicDay",
                        "OnTime",
                        "PartialHistory"});
#line 53
this.ScenarioInitialize(scenarioInfo);
            this.ScenarioStart();
#line 4
this.FeatureBackground();
#line hidden
            TechTalk.SpecFlow.Table table6 = new TechTalk.SpecFlow.Table(new string[] {
                        "LearnerId",
                        "PriceEpisodeIdentifier",
                        "Delivery Period",
                        "TransactionType",
                        "Amount"});
            table6.AddRow(new string[] {
                        "L1",
                        "p2",
                        "2",
                        "Completion (TT2)",
                        "3000"});
            table6.AddRow(new string[] {
                        "L2",
                        "p2",
                        "1",
                        "Learning (TT1)",
                        "800"});
            table6.AddRow(new string[] {
                        "L2",
                        "p2",
                        "2",
                        "Completion (TT2)",
                        "2400"});
#line 54
 testRunner.Given("the required payments component generates the following contract type 2 payable e" +
                    "arnings:", ((string)(null)), table6, "Given ");
#line 59
 testRunner.When("required payments event is received", ((string)(null)), ((TechTalk.SpecFlow.Table)(null)), "When ");
#line hidden
            TechTalk.SpecFlow.Table table7 = new TechTalk.SpecFlow.Table(new string[] {
                        "LearnerId",
                        "PriceEpisodeIdentifier",
                        "Delivery Period",
                        "TransactionType",
                        "FundingSource",
                        "Amount"});
            table7.AddRow(new string[] {
                        "L1",
                        "p2",
                        "2",
                        "Completion (TT2)",
                        "CoInvestedSfa (FS2)",
                        "2700"});
            table7.AddRow(new string[] {
                        "L1",
                        "p2",
                        "2",
                        "Completion (TT2)",
                        "CoInvestedEmployer (FS3)",
                        "300"});
            table7.AddRow(new string[] {
                        "L2",
                        "p2",
                        "1",
                        "Learning (TT1)",
                        "CoInvestedSfa (FS2)",
                        "720"});
            table7.AddRow(new string[] {
                        "L2",
                        "p2",
                        "1",
                        "Learning (TT1)",
                        "CoInvestedEmployer (FS3)",
                        "80"});
            table7.AddRow(new string[] {
                        "L2",
                        "p2",
                        "2",
                        "Completion (TT2)",
                        "CoInvestedSfa (FS2)",
                        "2160"});
            table7.AddRow(new string[] {
                        "L2",
                        "p2",
                        "2",
                        "Completion (TT2)",
                        "CoInvestedEmployer (FS3)",
                        "240"});
#line 60
 testRunner.Then("the payment source component will generate the following contract type 2 coinvest" +
                    "ed payments:", ((string)(null)), table7, "Then ");
#line hidden
            this.ScenarioCleanup();
        }
    }
}
#pragma warning restore
#endregion
