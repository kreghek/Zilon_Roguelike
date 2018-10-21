using System;

using Zilon.Core.Persons;
using Zilon.Core.Players;
using Zilon.Core.Props;
using Zilon.Core.Schemes;
using Zilon.Core.Tactics.Behaviour;
using Zilon.Core.Tactics.Spatial;

namespace Zilon.Core.Tactics
{
    public sealed class Actor : IActor
    {
        public event EventHandler Moved;
        public event EventHandler<OpenContainerEventArgs> OpenedContainer;
        public event EventHandler<UsedActEventArgs> UsedAct;
        public event EventHandler OnDefence;

        /// <inheritdoc />
        /// <summary>
        /// Песонаж, который лежит в основе актёра.
        /// </summary>
        public IPerson Person { get; }

        /// <summary>
        /// Текущий узел карты, в котором находится актёр.
        /// </summary>
        public IMapNode Node { get; set; }

        public float Damage { get; set; }

        public IPlayer Owner { get; }

        public Actor(IPerson person, IPlayer owner, IMapNode node)
        {
            Person = person;
            Owner = owner;
            Node = node;
        }

        public bool CanBeDamaged()
        {
            return !Person.Survival.IsDead;
        }

        public void MoveToNode(IMapNode targetNode)
        {
            Node = targetNode;
            Moved?.Invoke(this, new EventArgs());
        }

        public void OpenContainer(IPropContainer container, IOpenContainerMethod method)
        {
            var openResult = method.TryOpen(container);

            DoOpenContainer(openResult);
        }

        public void UseAct(IAttackTarget target, ITacticalAct tacticalAct)
        {
            DoUseAct(target, tacticalAct);
        }

        public override string ToString()
        {
            return $"{Person}";
        }

        public void UseProp(IProp usedProp)
        {
            var useData = usedProp.Scheme.Use;

            foreach (var rule in useData.CommonRules)
            {
                switch (rule.Type)
                {
                    case ConsumeCommonRuleType.Satiety:
                        Person.Survival.RestoreStat(SurvivalStatType.Satiety, 51);
                        break;

                    case ConsumeCommonRuleType.Thrist:
                        Person.Survival.RestoreStat(SurvivalStatType.Water, 51);
                        break;

                    case ConsumeCommonRuleType.Health:
                        Person.Survival.RestoreStat(SurvivalStatType.Health, 4);
                        break;
                }
            }

            if (useData.Consumable)
            {
                switch (usedProp)
                {
                    case Resource resource:
                        var removeResource = new Resource(resource.Scheme, 1);
                        Person.Inventory.Remove(removeResource);
                        break;
                }
            }
        }

        public void TakeDamage(int value)
        {
            Person.Survival.DecreaseStat(SurvivalStatType.Health, value);
        }

        public void ProcessDefence()
        {
            OnDefence?.Invoke(this, new EventArgs());
        }

        private void DoOpenContainer(IOpenContainerResult openResult)
        {
            var e = new OpenContainerEventArgs(openResult);
            OpenedContainer?.Invoke(this, e);
        }

        private void DoUseAct(IAttackTarget target, ITacticalAct tacticalAct)
        {
            var args = new UsedActEventArgs(target, tacticalAct);
            UsedAct?.Invoke(this, args);
        }
    }
}
