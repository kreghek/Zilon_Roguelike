using System;
using System.Collections.Generic;
using ReGoap.Core;
using Zilon.Core.WorldGeneration.AgentCards;
using Zilon.Core.WorldGeneration.LocalityStructures;

namespace Zilon.Core.WorldGeneration.AgentActions
{
    public sealed class BuildLocalityStructureAction: ReGoapActionBase<string, object>
    {
        private readonly Locality _locality;
        private readonly BasicLocalityStructure _localityStructure;

        public BuildLocalityStructureAction(Locality locality, BasicLocalityStructure localityStructure)
        {
            _locality = locality;
            _localityStructure = localityStructure;
        }

        public override bool CheckProceduralCondition(GoapActionStackData<string, object> stackData)
        {
            // Условия:
            // Город должен выдержать новое здание. Это значит, что баланс города должен покрывать потребности.
            // В городе должны быть ресурсы для начала строительства структуры.
            // В городе должно быть население, способное работать в новом здании.

            // Получаем текущий баланс города.
            // Если баланс города удовлетворяет требованям городской структуры,
            // то условия пройдены.

            // Учитываем только баланс с учётом других зданий.
            // На агентов - не рассчитываем.

            var hasEnounghBalance = CheckLocalityBalance(stackData);
            if (!hasEnounghBalance)
            {
                return false;
            }

            // Все условия этого действия проверены.
            // Проверяем ещё родительские.

            return base.CheckProceduralCondition(stackData);
        }

        private bool CheckLocalityBalance(GoapActionStackData<string, object> stackData)
        {
            foreach (var requiredResource in _localityStructure.RequiredResources)
            {
                if (stackData.settings.HasKey($"locality_{_locality.Name}_has_{requiredResource.Key}_balance"))
                {
                    var balance = stackData.settings.Get($"locality_{_locality.Name}_has_{requiredResource.Key}_balance") as int?;
                    if (balance.Value < requiredResource.Value)
                    {
                        return false;
                    }
                }
            }

            return true;
        }

        public override List<ReGoapState<string, object>> GetSettings(GoapActionStackData<string, object> stackData)
        {
            // Здесь должен быть ключ buildStructure.
            // Чтобы при построении плана от цели это действие
            // смогло быть сопоставлено с целью

            foreach (var pair in stackData.goalState.GetValues())
            {
                if (pair.Key.Contains("buildStructure"))
                {
                    //var resourceName = pair.Key.Substring(17);
                    //if (settingsPerResource.ContainsKey(resourceName))
                    //    return settingsPerResource[resourceName];
                    var results = new List<ReGoapState<string, object>>();
                    //settings.Set("resourceName", resourceName);
                    // push all available banks
                    //foreach (var banksPair in (Dictionary<Bank, Vector3>)stackData.currentState.Get("banks"))
                    //{
                    //    settings.Set("bank", banksPair.Key);
                    //    settings.Set("bankPosition", banksPair.Value);
                    //    results.Add(settings.Clone());
                    //}
                    //settingsPerResource[resourceName] = results;

                    // Не понятно, для чего это делается.
                    results.Add(settings.Clone());
                    return results;
                }
            }
            return base.GetSettings(stackData);
        }

        public override ReGoapState<string, object> GetEffects(GoapActionStackData<string, object> stackData)
        {
            // В качестве эффектов будет:
            // Изменение баланса города.
            // Указываем, что это здание есть в городе. Наличие здания в городе указываем через счётчик.
            // Указываем, что из города изымаются ресурсы, требуемые для начала строительства.

            foreach (var product in _localityStructure.ProductResources)
            {
                if (stackData.settings.HasKey($"locality_{_locality.Name}_has_{product.Key}_balance"))
                {
                    var balance = stackData.settings.Get($"locality_{_locality.Name}_has_{product.Key}_balance") as int?;
                    balance += product.Value;
                    effects.Set($"locality_{_locality.Name}_has_{product.Key}_balance", balance);
                }
            }

            effects.Set($"buildStructure_{_localityStructure.SpeciaName}_IN_{_locality.Name}", true);

            return effects;
        }

        public override ReGoapState<string, object> GetPreconditions(GoapActionStackData<string, object> stackData)
        {
            //if (stackData.settings.HasKey("bank"))
            //    preconditions.Set("isAtPosition", stackData.settings.Get("bankPosition"));
            //if (stackData.settings.HasKey("resourceName"))
            //    preconditions.Set("hasResource" + stackData.settings.Get("resourceName") as string, true);
            return preconditions;
        }

        public override void Run(
            IReGoapAction<string, object> previous,
            IReGoapAction<string, object> next,
            ReGoapState<string, object> settings,
            ReGoapState<string, object> goalState,
            Action<IReGoapAction<string, object>> done,
            Action<IReGoapAction<string, object>> fail)
        {
            base.Run(previous, next, settings, goalState, done, fail);
            this.settings = settings;
            //var bank = settings.Get("bank") as Bank;
            //if (bank != null && bank.AddResource(resourcesBag, (string)settings.Get("resourceName")))
            //{
            //    done(this);
            //}
            //else
            {
                fail(this);
            }
        }
    }
}
