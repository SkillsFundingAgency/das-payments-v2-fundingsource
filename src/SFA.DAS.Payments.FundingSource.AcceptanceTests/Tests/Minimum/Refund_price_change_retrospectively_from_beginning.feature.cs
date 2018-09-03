﻿// ------------------------------------------------------------------------------
//  <auto-generated>
//      This code was generated by SpecFlow (http://www.specflow.org/).
//      SpecFlow Version:1.9.0.77
//      SpecFlow Generator Version:1.9.0.0
//      Runtime Version:4.0.30319.42000
// 
//      Changes to this file may cause incorrect behavior and will be lost if
//      the code is regenerated.
//  </auto-generated>
// ------------------------------------------------------------------------------
#region Designer generated code
#pragma warning disable
namespace SFA.DAS.Payments.FundingSource.AcceptanceTests.Tests.Minimum
{
    using TechTalk.SpecFlow;
    
    
    [System.CodeDom.Compiler.GeneratedCodeAttribute("TechTalk.SpecFlow", "1.9.0.77")]
    [System.Runtime.CompilerServices.CompilerGeneratedAttribute()]
    [NUnit.Framework.TestFixtureAttribute()]
    [NUnit.Framework.DescriptionAttribute("Refunds - Provider earnings and payments where learner refund payments are due")]
    public partial class Refunds_ProviderEarningsAndPaymentsWhereLearnerRefundPaymentsAreDueFeature
    {
        
        private static TechTalk.SpecFlow.ITestRunner testRunner;
        
#line 1 "Refund_price_change_retrospectively_from_beginning.feature"
#line hidden
        
        [NUnit.Framework.TestFixtureSetUpAttribute()]
        public virtual void FeatureSetup()
        {
            testRunner = TechTalk.SpecFlow.TestRunnerManager.GetTestRunner();
            TechTalk.SpecFlow.FeatureInfo featureInfo = new TechTalk.SpecFlow.FeatureInfo(new System.Globalization.CultureInfo("en-US"), "Refunds - Provider earnings and payments where learner refund payments are due", " 894-AC02 - non DAS standard learner, payments made then price is changed retrosp" +
                    "ectively from beginning", ProgrammingLanguage.CSharp, ((string[])(null)));
            testRunner.OnFeatureStart(featureInfo);
        }
        
        [NUnit.Framework.TestFixtureTearDownAttribute()]
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
        
        public virtual void ScenarioSetup(TechTalk.SpecFlow.ScenarioInfo scenarioInfo)
        {
            testRunner.OnScenarioStart(scenarioInfo);
        }
        
        public virtual void ScenarioCleanup()
        {
            testRunner.CollectScenarioErrors();
        }
        
        public virtual void FeatureBackground()
        {
#line 9
#line 10
 testRunner.Given("the current processing period is 3", ((string)(null)), ((TechTalk.SpecFlow.Table)(null)), "Given ");
#line hidden
            TechTalk.SpecFlow.Table table1 = new TechTalk.SpecFlow.Table(new string[] {
                        "LearnRefNumber",
                        "Ukprn",
                        "ULN"});
            table1.AddRow(new string[] {
                        "learnref1",
                        "10000",
                        "10000"});
#line 12
 testRunner.And("the following learners:", ((string)(null)), table1, "And ");
#line hidden
            TechTalk.SpecFlow.Table table2 = new TechTalk.SpecFlow.Table(new string[] {
                        "PriceEpisodeIdentifier",
                        "Period",
                        "ULN",
                        "TransactionType",
                        "Amount"});
            table2.AddRow(new string[] {
                        "p1",
                        "1",
                        "10000",
                        "1",
                        "-750"});
            table2.AddRow(new string[] {
                        "p1",
                        "2",
                        "10000",
                        "1",
                        "-750"});
            table2.AddRow(new string[] {
                        "p2",
                        "1",
                        "10000",
                        "1",
                        "0.6667"});
            table2.AddRow(new string[] {
                        "p2",
                        "2",
                        "10000",
                        "1",
                        "0.6667"});
            table2.AddRow(new string[] {
                        "p2",
                        "3",
                        "10000",
                        "1",
                        "0.6667"});
#line 16
 testRunner.And("the payments due component generates the following contract type 2 on program ear" +
                    "nings:", ((string)(null)), table2, "And ");
#line hidden
        }
        
        [NUnit.Framework.TestAttribute()]
        [NUnit.Framework.DescriptionAttribute("Contract Type 2 Learning payment")]
        [NUnit.Framework.CategoryAttribute("Non-DAS")]
        [NUnit.Framework.CategoryAttribute("minimum_tests")]
        [NUnit.Framework.CategoryAttribute("Refunds")]
        [NUnit.Framework.CategoryAttribute("price_reduced_retrospectively")]
        public virtual void ContractType2LearningPayment()
        {
            TechTalk.SpecFlow.ScenarioInfo scenarioInfo = new TechTalk.SpecFlow.ScenarioInfo("Contract Type 2 Learning payment", new string[] {
                        "Non-DAS",
                        "minimum_tests",
                        "Refunds",
                        "price_reduced_retrospectively"});
#line 29
this.ScenarioSetup(scenarioInfo);
#line 9
this.FeatureBackground();
#line 31
 testRunner.When("MASH is received", ((string)(null)), ((TechTalk.SpecFlow.Table)(null)), "When ");
#line hidden
            TechTalk.SpecFlow.Table table3 = new TechTalk.SpecFlow.Table(new string[] {
                        "LearnRefNumber",
                        "Ukprn",
                        "PriceEpisodeIdentifier",
                        "Period",
                        "ULN",
                        "TransactionType",
                        "FundingSource",
                        "Amount"});
            table3.AddRow(new string[] {
                        "learnref1",
                        "10000",
                        "p1",
                        "1",
                        "10000",
                        "Learning_1",
                        "CoInvestedSfa_2",
                        "-675"});
            table3.AddRow(new string[] {
                        "learnref1",
                        "10000",
                        "p1",
                        "1",
                        "10000",
                        "Learning_1",
                        "CoInvestedEmployer_3",
                        "-75"});
            table3.AddRow(new string[] {
                        "learnref1",
                        "10000",
                        "p1",
                        "2",
                        "10000",
                        "Learning_1",
                        "CoInvestedSfa_2",
                        "-675"});
            table3.AddRow(new string[] {
                        "learnref1",
                        "10000",
                        "p1",
                        "2",
                        "10000",
                        "Learning_1",
                        "CoInvestedEmployer_3",
                        "-75"});
            table3.AddRow(new string[] {
                        "learnref1",
                        "10000",
                        "p2",
                        "1",
                        "10000",
                        "Learning_1",
                        "CoInvestedSfa_2",
                        "0.60"});
            table3.AddRow(new string[] {
                        "learnref1",
                        "10000",
                        "p2",
                        "1",
                        "10000",
                        "Learning_1",
                        "CoInvestedEmployer_3",
                        "0.0667"});
            table3.AddRow(new string[] {
                        "learnref1",
                        "10000",
                        "p2",
                        "2",
                        "10000",
                        "Learning_1",
                        "CoInvestedSfa_2",
                        "0.60"});
            table3.AddRow(new string[] {
                        "learnref1",
                        "10000",
                        "p2",
                        "2",
                        "10000",
                        "Learning_1",
                        "CoInvestedEmployer_3",
                        "0.0667"});
            table3.AddRow(new string[] {
                        "learnref1",
                        "10000",
                        "p2",
                        "3",
                        "10000",
                        "Learning_1",
                        "CoInvestedSfa_2",
                        "0.60"});
            table3.AddRow(new string[] {
                        "learnref1",
                        "10000",
                        "p2",
                        "3",
                        "10000",
                        "Learning_1",
                        "CoInvestedEmployer_3",
                        "0.0667"});
#line 33
 testRunner.Then("the payment source component will generate the following contract type 2 coinvest" +
                    "ed payments:", ((string)(null)), table3, "Then ");
#line hidden
            this.ScenarioCleanup();
        }
    }
}
#pragma warning restore
#endregion
