using Zilon.Core.PersonModules;
using Zilon.Core.Persons;
using Zilon.Core.Tactics;
using Zilon.Core.Tactics.Behaviour;
using Zilon.Core.Tactics.Spatial;

namespace Zilon.Bot.Players.Logics
{
    public sealed class DefeatTargetLogicState : LogicStateBase
    {
        private const int REFRESH_COUNTER_VALUE = 3;

        private readonly ITacticalActUsageService _actService;

        private MoveTask _moveTask;

        private int _refreshCounter;

        private IAttackTarget _target;

        public DefeatTargetLogicState(ITacticalActUsageService actService)
        {
            _actService = actService ?? throw new ArgumentNullException(nameof(actService));
        }

        private IAttackTarget GetTarget(IActor actor, ISectorMap _map, IActorManager actorManager)
        {
            //TODO Убрать дублирование кода с IntruderDetectedTrigger
            // Этот фрагмент уже однажды был использован неправильно,
            // что привело к трудноуловимой ошибке.
            var intruders = CheckForIntruders(actor, _map, actorManager);

            var orderedIntruders = intruders.OrderBy(x => _map.DistanceBetween(actor.Node, x.Node));
            var nearbyIntruder = orderedIntruders.FirstOrDefault();

            return nearbyIntruder;
        }

        private IEnumerable<IActor> CheckForIntruders(IActor actor, ISectorMap map, IActorManager actorManager)
        {
            foreach (var target in actorManager.Items)
            {
                if (target.Person.Fraction == actor.Person.Fraction)
                {
                    continue;
                }

                if (target.Person.CheckIsDead())
                {
                    continue;
                }

                var isVisible = LogicHelper.CheckTargetVisible(map, actor.Node, target.Node);
                if (!isVisible)
                {
                    continue;
                }

                yield return target;
            }
        }

        private AttackParams CheckAttackAvailability(IActor actor, IAttackTarget target, ISectorMap map)
        {
            if (actor.Person.GetModuleSafe<ICombatActModule>() is null)
            {
                throw new NotSupportedException();
            }

            IInventoryModule inventory = actor.Person.GetModuleSafe<IInventoryModule>();

            ITacticalAct act =
                SelectActHelper.SelectBestAct(actor.Person.GetModule<ICombatActModule>().CalcCombatActs(), inventory);

            var isInDistance = act.CheckDistance(actor.Node, target.Node, map);
            var targetIsOnLine = map.TargetIsOnLine(actor.Node, target.Node);

            AttackParams attackParams = new AttackParams
            {
                IsAvailable = isInDistance && targetIsOnLine, TacticalAct = act
            };

            return attackParams;
        }

        public override IActorTask GetTask(IActor actor, ISectorTaskSourceContext context,
            ILogicStrategyData strategyData)
        {
            if (_target == null)
            {
                _target = GetTarget(actor, context.Sector.Map, context.Sector.ActorManager);
            }

            var targetCanBeDamaged = _target.CanBeDamaged();
            if (!targetCanBeDamaged)
            {
                Complete = true;
                return null;
            }

            AttackParams attackParams = CheckAttackAvailability(actor, _target, context.Sector.Map);
            if (attackParams.IsAvailable)
            {
                ITacticalAct act = attackParams.TacticalAct;

                ActorTaskContext taskContext = new ActorTaskContext(context.Sector);

                AttackTask attackTask = new AttackTask(actor, taskContext, _target, act, _actService);
                return attackTask;
            }
            // Маршрут до цели обновляем каждые 3 хода.
            // Для оптимизации.
            // Эффект потери цели.

            if (_refreshCounter > 0 && _moveTask?.CanExecute() == true)
            {
                _refreshCounter--;
                return _moveTask;
            }

            ISectorMap map = context.Sector.Map;
            _refreshCounter = REFRESH_COUNTER_VALUE;
            var targetIsOnLine = map.TargetIsOnLine(actor.Node, _target.Node);

            if (targetIsOnLine)
            {
                ActorTaskContext taskContext = new ActorTaskContext(context.Sector);

                _moveTask = new MoveTask(actor, taskContext, _target.Node, map);
                return _moveTask;
            }

            // Цел за пределами видимости. Считается потерянной.
            return null;
        }

        protected override void ResetData()
        {
            _refreshCounter = 0;
            _target = null;
            _moveTask = null;
        }

        private class AttackParams
        {
            public bool IsAvailable { get; set; }
            public ITacticalAct TacticalAct { get; set; }
        }
    }
}