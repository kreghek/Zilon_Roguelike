using System;

using Zilon.Core.Components;
using Zilon.Core.Persons;
using Zilon.Core.Players;
using Zilon.Core.Props;
using Zilon.Core.Tactics.Behaviour;
using Zilon.Core.Tactics.Spatial;

namespace Zilon.Core.Tactics
{
    public sealed class Actor : IActor
    {
        public Actor(IPerson person, IPlayer owner, IMapNode node)
        {
            Person = person;
            Owner = owner;
            Node = node;
        }

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

        private void DoOpenContainer(IOpenContainerResult openResult)
        {
            var e = new OpenContainerEventArgs(openResult);
            OpenedContainer?.Invoke(this, e);
        }

        public event EventHandler Moved;

        public event EventHandler<OpenContainerEventArgs> OpenedContainer;

        public event EventHandler<UsedActEventArgs> UsedAct;
        public event EventHandler OnDefence;

        public void UseAct(IAttackTarget target, ITacticalAct tacticalAct)
        {
            DoUseAct(target, tacticalAct);
        }

        private void DoUseAct(IAttackTarget target, ITacticalAct tacticalAct)
        {
            var args = new UsedActEventArgs(target, tacticalAct);
            UsedAct?.Invoke(this, args);
        }

        public override string ToString()
        {
            return $"{Person}";
        }

        public void UseProp(IProp usedProp)
        {
            var useData = usedProp.Scheme.Use;

            var efficient = 50;
            if (useData.Value > 0)
            {
                efficient = useData.Value;
            }

            foreach (var rule in useData.CommonRules)
            {
                switch (rule)
                {
                    case ConsumeCommonRule.Satiety:
                        Person.Survival.RestoreStat(SurvivalStatType.Satiety, efficient);
                        break;

                    case ConsumeCommonRule.Thrist:
                        Person.Survival.RestoreStat(SurvivalStatType.Water, efficient);
                        break;

                    case ConsumeCommonRule.Health:
                        Person.Survival.RestoreStat(SurvivalStatType.Health, efficient);
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
    }
}
