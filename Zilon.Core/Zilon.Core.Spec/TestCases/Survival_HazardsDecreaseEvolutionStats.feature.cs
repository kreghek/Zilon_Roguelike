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
namespace Zilon.Core.Spec.TestCases
{
    using TechTalk.SpecFlow;
    
    
    [System.CodeDom.Compiler.GeneratedCodeAttribute("TechTalk.SpecFlow", "2.4.0.0")]
    [System.Runtime.CompilerServices.CompilerGeneratedAttribute()]
    [TechTalk.SpecRun.FeatureAttribute("Survival_HazardsDecreaseEvolutionStats", Description="\tЧтобы избегать получение угроз выживания (голод/жажда)\r\n\tКак игроку\r\n\tМне нужно," +
        " чтобы угрозы снижали характеристики актёра, пока актёр от них не избавиться.", SourceFile="TestCases\\Survival_HazardsDecreaseEvolutionStats.feature", SourceLine=0)]
    public partial class Survival_HazardsDecreaseEvolutionStatsFeature
    {
        
        private TechTalk.SpecFlow.ITestRunner testRunner;
        
#line 1 "Survival_HazardsDecreaseEvolutionStats.feature"
#line hidden
        
        [TechTalk.SpecRun.FeatureInitialize()]
        public virtual void FeatureSetup()
        {
            testRunner = TechTalk.SpecFlow.TestRunnerManager.GetTestRunner();
            TechTalk.SpecFlow.FeatureInfo featureInfo = new TechTalk.SpecFlow.FeatureInfo(new System.Globalization.CultureInfo("en-US"), "Survival_HazardsDecreaseEvolutionStats", "\tЧтобы избегать получение угроз выживания (голод/жажда)\r\n\tКак игроку\r\n\tМне нужно," +
                    " чтобы угрозы снижали характеристики актёра, пока актёр от них не избавиться.", ProgrammingLanguage.CSharp, ((string[])(null)));
            testRunner.OnFeatureStart(featureInfo);
        }
        
        [TechTalk.SpecRun.FeatureCleanup()]
        public virtual void FeatureTearDown()
        {
            testRunner.OnFeatureEnd();
            testRunner = null;
        }
        
        public virtual void TestInitialize()
        {
        }
        
        [TechTalk.SpecRun.ScenarioCleanup()]
        public virtual void ScenarioTearDown()
        {
            testRunner.OnScenarioEnd();
        }
        
        public virtual void ScenarioInitialize(TechTalk.SpecFlow.ScenarioInfo scenarioInfo)
        {
            testRunner.OnScenarioInitialize(scenarioInfo);
        }
        
        public virtual void ScenarioStart()
        {
            testRunner.OnScenarioStart();
        }
        
        public virtual void ScenarioCleanup()
        {
            testRunner.CollectScenarioErrors();
        }
        
        public virtual void УгрозыВыживанияСнижаютХарактеристикиМодуляСраженияУАктёраИгрока_(string mapSize, string personSid, string actorNodeX, string actorNodeY, string startEffect, string combatStat, string combatStatValue, string[] exampleTags)
        {
            string[] @__tags = new string[] {
                    "survival",
                    "dev0"};
            if ((exampleTags != null))
            {
                @__tags = System.Linq.Enumerable.ToArray(System.Linq.Enumerable.Concat(@__tags, exampleTags));
            }
            TechTalk.SpecFlow.ScenarioInfo scenarioInfo = new TechTalk.SpecFlow.ScenarioInfo("Угрозы выживания снижают характеристики модуля сражения у актёра игрока.", null, @__tags);
#line 7
this.ScenarioInitialize(scenarioInfo);
            this.ScenarioStart();
#line 8
 testRunner.Given(string.Format("Есть карта размером {0}", mapSize), ((string)(null)), ((TechTalk.SpecFlow.Table)(null)), "Given ");
#line 9
 testRunner.And(string.Format("Есть актёр игрока класса {0} в ячейке ({1}, {2})", personSid, actorNodeX, actorNodeY), ((string)(null)), ((TechTalk.SpecFlow.Table)(null)), "And ");
#line 10
 testRunner.And(string.Format("Актёр имеет эффект {0}", startEffect), ((string)(null)), ((TechTalk.SpecFlow.Table)(null)), "And ");
#line 11
 testRunner.Then(string.Format("Актёр имеет характристику модуля сражения {0} равную {1}", combatStat, combatStatValue), ((string)(null)), ((TechTalk.SpecFlow.Table)(null)), "Then ");
#line hidden
            this.ScenarioCleanup();
        }
        
        [TechTalk.SpecRun.ScenarioAttribute("Угрозы выживания снижают характеристики модуля сражения у актёра игрока., Variant" +
            " 0", new string[] {
                "survival",
                "dev0"}, SourceLine=14)]
        public virtual void УгрозыВыживанияСнижаютХарактеристикиМодуляСраженияУАктёраИгрока__Variant0()
        {
#line 7
this.УгрозыВыживанияСнижаютХарактеристикиМодуляСраженияУАктёраИгрока_("2", "captain", "0", "0", "Слабый голод", "melee", "9", ((string[])(null)));
#line hidden
        }
        
        [TechTalk.SpecRun.ScenarioAttribute("Угрозы выживания снижают характеристики модуля сражения у актёра игрока., Variant" +
            " 1", new string[] {
                "survival",
                "dev0"}, SourceLine=14)]
        public virtual void УгрозыВыживанияСнижаютХарактеристикиМодуляСраженияУАктёраИгрока__Variant1()
        {
#line 7
this.УгрозыВыживанияСнижаютХарактеристикиМодуляСраженияУАктёраИгрока_("2", "captain", "0", "0", "Голод", "melee", "7", ((string[])(null)));
#line hidden
        }
        
        [TechTalk.SpecRun.ScenarioAttribute("Угрозы выживания снижают характеристики модуля сражения у актёра игрока., Variant" +
            " 2", new string[] {
                "survival",
                "dev0"}, SourceLine=14)]
        public virtual void УгрозыВыживанияСнижаютХарактеристикиМодуляСраженияУАктёраИгрока__Variant2()
        {
#line 7
this.УгрозыВыживанияСнижаютХарактеристикиМодуляСраженияУАктёраИгрока_("2", "captain", "0", "0", "Голодание", "melee", "5", ((string[])(null)));
#line hidden
        }
        
        [TechTalk.SpecRun.ScenarioAttribute("Угрозы выживания снижают характеристики модуля сражения у актёра игрока., Variant" +
            " 3", new string[] {
                "survival",
                "dev0"}, SourceLine=14)]
        public virtual void УгрозыВыживанияСнижаютХарактеристикиМодуляСраженияУАктёраИгрока__Variant3()
        {
#line 7
this.УгрозыВыживанияСнижаютХарактеристикиМодуляСраженияУАктёраИгрока_("2", "captain", "0", "0", "Слабая жажда", "melee", "9", ((string[])(null)));
#line hidden
        }
        
        [TechTalk.SpecRun.ScenarioAttribute("Угрозы выживания снижают характеристики модуля сражения у актёра игрока., Variant" +
            " 4", new string[] {
                "survival",
                "dev0"}, SourceLine=14)]
        public virtual void УгрозыВыживанияСнижаютХарактеристикиМодуляСраженияУАктёраИгрока__Variant4()
        {
#line 7
this.УгрозыВыживанияСнижаютХарактеристикиМодуляСраженияУАктёраИгрока_("2", "captain", "0", "0", "Жажда", "melee", "7", ((string[])(null)));
#line hidden
        }
        
        [TechTalk.SpecRun.ScenarioAttribute("Угрозы выживания снижают характеристики модуля сражения у актёра игрока., Variant" +
            " 5", new string[] {
                "survival",
                "dev0"}, SourceLine=14)]
        public virtual void УгрозыВыживанияСнижаютХарактеристикиМодуляСраженияУАктёраИгрока__Variant5()
        {
#line 7
this.УгрозыВыживанияСнижаютХарактеристикиМодуляСраженияУАктёраИгрока_("2", "captain", "0", "0", "Обезвоживание", "melee", "5", ((string[])(null)));
#line hidden
        }
        
        [TechTalk.SpecRun.TestRunCleanup()]
        public virtual void TestRunCleanup()
        {
            TechTalk.SpecFlow.TestRunnerManager.GetTestRunner().OnTestRunEnd();
        }
    }
}
#pragma warning restore
#endregion
