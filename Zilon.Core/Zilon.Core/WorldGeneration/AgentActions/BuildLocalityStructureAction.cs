using System;
using System.Collections.Generic;
using System.Linq;
using ReGoap.Core;

namespace Zilon.Core.WorldGeneration.AgentActions
{
    public sealed class BuildLocalityStructureAction: ReGoapActionBase<string, object>
    {
        private readonly Locality _locality;
        private readonly ILocalityStructure _localityStructure;

        private List<ReGoapState<string, object>> _settingsList;

        public BuildLocalityStructureAction(Locality locality, ILocalityStructure localityStructure)
        {
            _settingsList = new List<ReGoapState<string, object>>();

            _locality = locality;
            _localityStructure = localityStructure;

            // precond
            //foreach (var requiredResource in _localityStructure.RequiredResources)
            //{
            //    preconditions.Set($"locality_{_locality.Name}_has_{requiredResource.Key}_balance", requiredResource.Value);
            //}

            // precond
            foreach (var requiredResource in _localityStructure.RequiredResources)
            {
                preconditions.Set($"locality_{_locality.Name}_has_{requiredResource.Key}_enought_balance", true);
            }

            // effects
            //foreach (var product in _localityStructure.ProductResources)
            //{
            //    effects.Set($"locality_{_locality.Name}_has_{product.Key}_balance", product.Value);

            //    //if (stackData.settings.HasKey($"locality_{_locality.Name}_has_{product.Key}_balance"))
            //    //{
            //    //    var balance = stackData.settings.Get($"locality_{_locality.Name}_has_{product.Key}_balance") as int?;
            //    //    balance += product.Value;
            //    //    effects.Set($"locality_{_locality.Name}_has_{product.Key}_balance", balance);
            //    //}
            //}

            effects.Set($"structure_{_localityStructure.Name}_in_{_locality.Name}", true);
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
            if (_settingsList.Count == 0)
                CalculateSettingsList(stackData);
            return _settingsList;



            //// Похоже, что здесь подготавливается состояние, которое нужно текущеу действию.
            //// Состояние можно готовить на основе переданного - текущее.
            //var results = new List<ReGoapState<string, object>>() {
            //    stackData.currentState.Clone()
            //};
            //return results;

            //foreach (var pair in stackData.goalState.GetValues())
            //{

            //    if (pair.Key.Contains("buildStructure"))
            //    {
            //        var clone = settings.Clone();
            //        //var resourceName = pair.Key.Substring(17);
            //        //if (settingsPerResource.ContainsKey(resourceName))
            //        //    return settingsPerResource[resourceName];
            //        var results = new List<ReGoapState<string, object>>();
            //        //settings.Set("resourceName", resourceName);
            //        // push all available banks
            //        //foreach (var banksPair in (Dictionary<Bank, Vector3>)stackData.currentState.Get("banks"))
            //        //{
            //        //    settings.Set("bank", banksPair.Key);
            //        //    settings.Set("bankPosition", banksPair.Value);
            //        //    results.Add(settings.Clone());
            //        //}
            //        //settingsPerResource[resourceName] = results;

            //        // Не понятно, для чего это делается.
            //        results.Add(settings.Clone());
            //        return results;
            //    }
            //}
            //return base.GetSettings(stackData);
        }

        private void CalculateSettingsList(GoapActionStackData<string, object> stackData)
        {
            _settingsList.Clear();

            // Всё ещё не совсем понятно, как использовать этот метод.
            // Здесь в настройки указываем, достаточно ли ресурсов для выполнения действия.
            // Чтобы дальше это могло биться с предусловиями.

            foreach (var requiredResource in _localityStructure.RequiredResources)
            {
                if (stackData.currentState.HasKey($"locality_{_locality.Name}_has_{requiredResource.Key}_balance"))
                {
                    var balance = stackData.currentState.Get($"locality_{_locality.Name}_has_{requiredResource.Key}_balance") as int?;
                    if (balance.Value >= requiredResource.Value)
                    {
                        stackData.currentState.Set($"locality_{_locality.Name}_has_{requiredResource.Key}_enought_balance", true);
                    }
                }
            }

            _settingsList.Add(settings.Clone());
        }

        //public override ReGoapState<string, object> GetEffects(GoapActionStackData<string, object> stackData)
        //{
        //    effects.Clear();

        //    // В качестве эффектов будет:
        //    // Изменение баланса города.
        //    // Указываем, что это здание есть в городе. Наличие здания в городе указываем через счётчик.
        //    // Указываем, что из города изымаются ресурсы, требуемые для начала строительства.

        //    foreach (var product in _localityStructure.ProductResources)
        //    {
        //        if (stackData.settings.HasKey($"locality_{_locality.Name}_has_{product.Key}_balance"))
        //        {
        //            var balance = stackData.settings.Get($"locality_{_locality.Name}_has_{product.Key}_balance") as int?;
        //            balance += product.Value;
        //            effects.Set($"locality_{_locality.Name}_has_{product.Key}_balance", balance);
        //        }
        //    }

        //    effects.Set($"structure_{_localityStructure.Name}_in_{_locality.Name}", true);

        //    return effects;
        //}

        //public override ReGoapState<string, object> GetPreconditions(GoapActionStackData<string, object> stackData)
        //{
        //    // Как минимум в одном случае:
        //    // Этот метод на вход принимает состояние мира.
        //    // На выход выдаёт предусловия действия в объекте состояния.
        //    // Затем расчитывается разница между этим состоянием и состоянием мира.
        //    // Разница должна быть нулевой, чтобы считать текущее действие - как финишное действие плана.

        //    // То есть в текущем методе мы из состояния мира
        //    // вырезаем все ключи, оставляя только те, которые бьются предусловиями.

        //    var keys = _localityStructure.RequiredResources.Select(x => $"locality_{_locality.Name}_has_{x.Key}_balance");

        //    var precond = stackData.currentState.Clone();
        //    foreach (var pair in precond.GetValues().ToArray())
        //    {
        //        if (!keys.Contains(pair.Key))
        //        {
        //            precond.Remove(pair.Key);
        //        }
        //    }

        //    foreach (var requiredResource in _localityStructure.RequiredResources)
        //    {
        //        var localityBalance = (int?)precond.Get($"locality_{_locality.Name}_has_{requiredResource.Key}_balance");
        //        if (localityBalance == null || localityBalance.Value < requiredResource.Value)
        //        {
        //            precond.Remove($"locality_{_locality.Name}_has_{requiredResource.Key}_balance");
        //        }
        //    }

        //    return precond;
        //}

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
