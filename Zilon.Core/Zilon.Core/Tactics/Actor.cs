using System;
using System.Diagnostics.CodeAnalysis;

using JetBrains.Annotations;

using Zilon.Core.Common;
using Zilon.Core.Components;
using Zilon.Core.Graphs;
using Zilon.Core.Persons;
using Zilon.Core.Players;
using Zilon.Core.Props;
using Zilon.Core.Schemes;
using Zilon.Core.Tactics.Behaviour;

namespace Zilon.Core.Tactics
{
    public sealed class Actor : IActor
    {
        private readonly IPerkResolver _perkResolver;

        private int _gameLoopCounter;

        /// <inheritdoc/>
        public event EventHandler Moved;

        /// <inheritdoc/>
        public event EventHandler<OpenContainerEventArgs> OpenedContainer;

        /// <inheritdoc/>
        public event EventHandler<UsedActEventArgs> UsedAct;

        /// <inheritdoc/>
        public event EventHandler<DamageTakenEventArgs> DamageTaken;

        /// <inheritdoc/>
        public event EventHandler<UsedPropEventArgs> UsedProp;

        /// <inheritdoc />
        /// <summary>
        /// Песонаж, который лежит в основе актёра.
        /// </summary>
        public IPerson Person { get; }

        /// <summary>
        /// Текущий узел карты, в котором находится актёр.
        /// </summary>
        public IGraphNode Node { get; private set; }

        public IPlayer Owner { get; }
        public ISectorFowData SectorFowData { get; }
        public PhysicalSize PhysicalSize { get => Person.PhysicalSize; }
        public int GameLoopCounter { get => _gameLoopCounter; }

        [ExcludeFromCodeCoverage]
        public Actor([NotNull] IPerson person, [NotNull]  IPlayer owner, [NotNull]  IGraphNode node)
        {
            Person = person ?? throw new ArgumentNullException(nameof(person));
            Owner = owner ?? throw new ArgumentNullException(nameof(owner));
            Node = node ?? throw new ArgumentNullException(nameof(node));

            if (SectorFowData == null)
            {
                SectorFowData = new MonsterSectorFowData();
            }
        }

        public Actor([NotNull] IPerson person, [NotNull]  IPlayer owner, [NotNull]  IGraphNode node,
            [CanBeNull] IPerkResolver perkResolver) : this(person, owner, node)
        {
            _perkResolver = perkResolver;
        }

        public Actor([NotNull] IPerson person, [NotNull]  IPlayer owner, [NotNull]  IGraphNode node,
            [CanBeNull] IPerkResolver perkResolver, [CanBeNull] ISectorFowData sectorFowData) : this(person, owner, node)
        {
            _perkResolver = perkResolver;

            SectorFowData = sectorFowData;
        }

        public bool CanBeDamaged()
        {
            if (Person.Survival == null)
            {
                return false;
            }

            return !Person.Survival.IsDead;
        }

        public void MoveToNode(IGraphNode targetNode)
        {
            Node = targetNode;
            Moved?.Invoke(this, new EventArgs());
        }

        public void OpenContainer(IPropContainer container, IOpenContainerMethod method)
        {
            var openResult = method?.TryOpen(container);

            DoOpenContainer(container, openResult);
        }

        public void UseAct(IAttackTarget target, ITacticalAct tacticalAct)
        {
            DoUseAct(target, tacticalAct);
        }

        public void UseProp(IProp usedProp)
        {
            if (usedProp is null)
            {
                throw new ArgumentNullException(nameof(usedProp));
            }

            var useData = usedProp.Scheme.Use;

            foreach (var rule in useData.CommonRules)
            {
                switch (rule.Direction)
                {
                    // Если направление не указано, то будет считаться положительное значение
                    //TODO При десериализации указывать значение Positive по умолчанию
                    default:
                    case PersonRuleDirection.Positive:
                        ProcessPositiveRule(rule.Type, rule.Level);
                        break;
                    case PersonRuleDirection.Negative:
                        ProcessNegativeRule(rule.Type, rule.Level);
                        break;
                }

            }

            if (useData.Consumable && Person.Inventory != null)
            {
                ConsumeResource(usedProp);

                if (_perkResolver != null)
                {
                    var consumeProgress = new ConsumeProviantJobProgress();
                    _perkResolver.ApplyProgress(consumeProgress, Person.EvolutionData);
                }
            }

            UsedProp?.Invoke(this, new UsedPropEventArgs(usedProp));
        }

        private void ProcessNegativeRule(ConsumeCommonRuleType type, PersonRuleLevel ruleLevel)
        {
            switch (type)
            {
                case ConsumeCommonRuleType.Satiety:
                    DecreaseStat(SurvivalStatType.Satiety, ruleLevel);
                    break;

                case ConsumeCommonRuleType.Thirst:
                    DecreaseStat(SurvivalStatType.Hydration, ruleLevel);
                    break;

                case ConsumeCommonRuleType.Health:
                    DecreaseStat(SurvivalStatType.Health, ruleLevel);
                    break;

                case ConsumeCommonRuleType.Intoxication:
                    DecreaseStat(SurvivalStatType.Intoxication, ruleLevel);
                    break;

                case ConsumeCommonRuleType.Undefined:
                default:
                    throw new ArgumentOutOfRangeException(nameof(type), $"Значение {type} не поддерживается.");
            }
        }

        private void ProcessPositiveRule(ConsumeCommonRuleType type, PersonRuleLevel ruleLevel)
        {
            switch (type)
            {
                case ConsumeCommonRuleType.Satiety:
                    RestoreStat(SurvivalStatType.Satiety, ruleLevel);
                    break;

                case ConsumeCommonRuleType.Thirst:
                    RestoreStat(SurvivalStatType.Hydration, ruleLevel);
                    break;

                case ConsumeCommonRuleType.Health:
                    RestoreStat(SurvivalStatType.Health, ruleLevel);
                    break;

                case ConsumeCommonRuleType.Intoxication:
                    RiseStat(SurvivalStatType.Intoxication, ruleLevel);
                    break;

                case ConsumeCommonRuleType.Undefined:
                default:
                    throw new ArgumentOutOfRangeException(nameof(type), $"Значение {type} не поддерживается.");
            }
        }

        private void ConsumeResource(IProp usedProp)
        {
            switch (usedProp)
            {
                case Resource resource:
                    var removeResource = new Resource(resource.Scheme, 1);
                    Person.Inventory.Remove(removeResource);
                    break;

                case Equipment equipment:
                    Person.Inventory.Remove(equipment);
                    break;
            }
        }

        public void TakeDamage(int value)
        {
            Person.Survival?.DecreaseStat(SurvivalStatType.Health, value);

            if (_perkResolver != null && Person.EvolutionData != null)
            {
                var takeDamageProgress = new TakeDamageJobProgress(value);
                _perkResolver.ApplyProgress(takeDamageProgress, Person.EvolutionData);

                var takeHitProgress = new TakeHitJobProgress();
                _perkResolver.ApplyProgress(takeHitProgress, Person.EvolutionData);
            }

            DoDamageTaken(value);
        }

        [ExcludeFromCodeCoverage]
        private void DoDamageTaken(int value)
        {
            DamageTaken?.Invoke(this, new DamageTakenEventArgs(value));
        }


        [ExcludeFromCodeCoverage]
        private void DoOpenContainer(IPropContainer container, IOpenContainerResult openResult)
        {
            var e = new OpenContainerEventArgs(container, openResult);
            OpenedContainer?.Invoke(this, e);
        }

        [ExcludeFromCodeCoverage]
        private void DoUseAct(IAttackTarget target, ITacticalAct tacticalAct)
        {
            var args = new UsedActEventArgs(target, tacticalAct);
            UsedAct?.Invoke(this, args);
        }

        [ExcludeFromCodeCoverage]
        public override string ToString()
        {
            return $"{Person}";
        }

        /// <summary>
        /// Метод введён специально для повышения уровня интоксикации.
        /// Так как глупо выглядит ResToreStat для повышения интоксикации.
        /// Просто семантически более удобная обёртка.
        /// </summary>
        /// <param name="statType"> Характеристика, повышаемая методом. </param>
        /// <param name="level"> Уровень увеличения. </param>
        private void RiseStat(SurvivalStatType statType, PersonRuleLevel level)
        {
            RestoreStat(statType, level);
        }

        private void RestoreStat(SurvivalStatType statType, PersonRuleLevel level)
        {
            switch (statType)
            {
                case SurvivalStatType.Satiety:
                case SurvivalStatType.Hydration:
                    RestoreSurvivalStatInner(statType, level);
                    break;

                case SurvivalStatType.Intoxication:
                    RiseIntoxicationLevel(level);
                    break;

                case SurvivalStatType.Health:
                    RestoreHp(level);
                    break;
            }
        }

        private void RestoreSurvivalStatInner(SurvivalStatType statType, PersonRuleLevel level)
        {
            switch (level)
            {
                case PersonRuleLevel.Lesser:
                    Person.Survival?.RestoreStat(statType,
                        PropMetrics.SurvivalLesserRestoreValue + 1);
                    break;

                case PersonRuleLevel.Normal:
                    Person.Survival?.RestoreStat(statType,
                        PropMetrics.SurvivalNormalRestoreValue + 1);
                    break;

                case PersonRuleLevel.Grand:
                    Person.Survival?.RestoreStat(statType,
                        PropMetrics.SurvivalGrandRestoreValue + 1);
                    break;

                case PersonRuleLevel.None:
                    throw new NotSupportedException();

                case PersonRuleLevel.Absolute:
                    throw new NotSupportedException();

                default:
                    throw new InvalidOperationException($"Неизвестный уровень влияния правила {level}.");
            }
        }

        private void RestoreHp(PersonRuleLevel level)
        {
            switch (level)
            {
                case PersonRuleLevel.Lesser:
                    Person.Survival?.RestoreStat(SurvivalStatType.Health,
                        PropMetrics.HpLesserRestoreValue);
                    break;

                case PersonRuleLevel.Normal:
                    Person.Survival?.RestoreStat(SurvivalStatType.Health,
                        PropMetrics.HpNormalRestoreValue);
                    break;

                case PersonRuleLevel.Grand:
                    Person.Survival?.RestoreStat(SurvivalStatType.Health,
                        PropMetrics.HpGrandRestoreValue);
                    break;

                default:
                    throw new InvalidOperationException($"Неизвестный уровень влияния правила {level}.");
            }
        }

        private void RiseIntoxicationLevel(PersonRuleLevel level)
        {
            switch (level)
            {
                case PersonRuleLevel.Lesser:
                    Person.Survival?.RestoreStat(SurvivalStatType.Intoxication,
                        PropMetrics.INTOXICATION_RISE_LESSER_VALUE + 1);
                    break;

                case PersonRuleLevel.Normal:
                    Person.Survival?.RestoreStat(SurvivalStatType.Intoxication,
                        PropMetrics.INTOXICATION_RISE_NORMAL_VALUE + 1);
                    break;

                case PersonRuleLevel.Grand:
                    Person.Survival?.RestoreStat(SurvivalStatType.Intoxication,
                        PropMetrics.INTOXICATION_RISE_GRAND_VALUE + 1);
                    break;

                default:
                    throw new InvalidOperationException($"Неизвестный уровень влияния правила {level}.");
            }
        }

        private void DecreaseStat(SurvivalStatType statType, PersonRuleLevel level)
        {
            switch (statType)
            {
                case SurvivalStatType.Satiety:
                case SurvivalStatType.Hydration:
                case SurvivalStatType.Intoxication:
                    DecreaseSurvivalStatInner(statType, level);
                    break;

                case SurvivalStatType.Health:
                    DecreaseHp(level);
                    break;
            }
        }

        private void DecreaseSurvivalStatInner(SurvivalStatType statType, PersonRuleLevel level)
        {
            switch (level)
            {
                case PersonRuleLevel.Lesser:
                    Person.Survival?.DecreaseStat(statType,
                        PropMetrics.SurvivalLesserRestoreValue - 1);
                    break;

                case PersonRuleLevel.Normal:
                    Person.Survival?.DecreaseStat(statType,
                        PropMetrics.SurvivalNormalRestoreValue - 1);
                    break;

                case PersonRuleLevel.Grand:
                    Person.Survival?.DecreaseStat(statType,
                        PropMetrics.SurvivalGrandRestoreValue - 1);
                    break;

                case PersonRuleLevel.None:
                    throw new NotSupportedException();

                case PersonRuleLevel.Absolute:
                    throw new NotSupportedException();

                default:
                    throw new InvalidOperationException($"Неизвестный уровень влияния правила {level}.");
            }
        }

        private void DecreaseHp(PersonRuleLevel level)
        {
            switch (level)
            {
                case PersonRuleLevel.Lesser:
                    Person.Survival?.DecreaseStat(SurvivalStatType.Health,
                        PropMetrics.HpLesserRestoreValue);
                    break;

                case PersonRuleLevel.Normal:
                    Person.Survival?.DecreaseStat(SurvivalStatType.Health,
                        PropMetrics.HpNormalRestoreValue);
                    break;

                case PersonRuleLevel.Grand:
                    Person.Survival?.DecreaseStat(SurvivalStatType.Health,
                        PropMetrics.HpGrandRestoreValue);
                    break;

                default:
                    throw new InvalidOperationException($"Неизвестный уровень влияния правила {level}.");
            }
        }

        public void IncreaseGameLoopCounter(int value)
        {
            _gameLoopCounter += value;
            if (_gameLoopCounter >= 1000)
            {
                _gameLoopCounter -= 1000;
            }
        }
    }
}
