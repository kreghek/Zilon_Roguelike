using System;
using System.Linq;

using Zilon.Core.Client;
using Zilon.Core.Common;
using Zilon.Core.PersonModules;
using Zilon.Core.Players;
using Zilon.Core.Props;
using Zilon.Core.StaticObjectModules;
using Zilon.Core.Tactics;
using Zilon.Core.Tactics.Behaviour;

namespace Zilon.Core.Commands.Sector
{
    public sealed class MineDepositCommand : ActorCommandBase
    {
        private readonly IMineDepositMethodRandomSource _mineDepositMethodRandomSource;
        private readonly IPlayer _player;

        public MineDepositCommand(
            IPlayer player,
            ISectorUiState playerState,
            IMineDepositMethodRandomSource mineDepositMethodRandomSource) : base(playerState)
        {
            _player = player;
            _mineDepositMethodRandomSource = mineDepositMethodRandomSource;
        }

        public override CanExecuteCheckResult CanExecute()
        {
            var selectedViewModel = PlayerState.SelectedViewModel ?? PlayerState.HoverViewModel;
            var staticObject = (selectedViewModel as IContainerViewModel)?.StaticObject;
            if (staticObject is null)
            {
                return new CanExecuteCheckResult { IsSuccess = false };
            }

            var sector = _player.SectorNode.Sector;
            if (sector is null)
            {
                throw new InvalidOperationException();
            }

            var map = sector.Map;

            var actor = PlayerState.ActiveActor?.Actor;
            if (actor is null)
            {
                throw new InvalidOperationException();
            }

            var currentNode = actor.Node;

            var distance = map.DistanceBetween(currentNode, staticObject.Node);
            if (distance > 1)
            {
                return new CanExecuteCheckResult { IsSuccess = false };
            }

            var targetDeposit = staticObject.GetModuleSafe<IPropDepositModule>();

            if (targetDeposit is null)
            {
                return new CanExecuteCheckResult { IsSuccess = false };
            }

            var equipmentCarrier = actor.Person.GetModuleSafe<IEquipmentModule>();
            if (equipmentCarrier is null)
            {
                return new CanExecuteCheckResult { IsSuccess = false };
            }

            var requiredTags = targetDeposit.GetToolTags();
            if (requiredTags.Any())
            {
                var equipedTool = GetEquipedTool(equipmentCarrier, requiredTags);
                if (equipedTool is null)
                {
                    return new CanExecuteCheckResult { IsSuccess = false };
                }

                return new CanExecuteCheckResult { IsSuccess = true };
            }

            // Если для добычи не указаны теги, то предполагается,
            // что добывать можно "руками".
            // То есть никакого инструмента не требуется.
            return new CanExecuteCheckResult { IsSuccess = true };
        }

        protected override void ExecuteTacticCommand()
        {
            var targetStaticObject = (PlayerState?.SelectedViewModel as IContainerViewModel)?.StaticObject;
            if (targetStaticObject is null)
            {
                throw new InvalidOperationException();
            }

            var targetDeposit = targetStaticObject.GetModule<IPropDepositModule>();

            var actor = PlayerState?.ActiveActor?.Actor;
            if (actor is null)
            {
                throw new InvalidOperationException();
            }

            var taskSource = PlayerState?.TaskSource;
            if (taskSource is null)
            {
                throw new InvalidOperationException();
            }

            var equipmentCarrier = actor.Person.GetModule<IEquipmentModule>();
            var requiredTags = targetDeposit.GetToolTags();

            if (requiredTags.Any())
            {
                var equipedTool = GetEquipedTool(equipmentCarrier, requiredTags);
                if (equipedTool is null)
                {
                    throw new InvalidOperationException("Try to mine without required tools.");
                }

                var intetion = new Intention<MineTask>(actor =>
                    CreateTaskByInstrument(actor, targetStaticObject, equipedTool));
                taskSource.Intent(intetion, actor);
            }
            else
            {
                // Добыча руками, если никаких тегов инструмента не задано.
                var intetion = new Intention<MineTask>(actor => CreateTaskByHands(actor, targetStaticObject));
                taskSource.Intent(intetion, actor);
            }
        }

        private MineTask CreateTaskByHands(IActor actor, IStaticObject staticObject)
        {
            var handMineDepositMethod = new HandMineDepositMethod(_mineDepositMethodRandomSource);

            var sector = _player.SectorNode.Sector;
            if (sector is null)
            {
                throw new InvalidOperationException();
            }

            var taskContext = new ActorTaskContext(sector);
            return new MineTask(actor, taskContext, staticObject, handMineDepositMethod);
        }

        private MineTask CreateTaskByInstrument(IActor actor, IStaticObject staticObject, Equipment equipedTool)
        {
            var toolMineDepositMethod = new ToolMineDepositMethod(equipedTool, _mineDepositMethodRandomSource);

            var sector = _player.SectorNode.Sector;
            if (sector is null)
            {
                throw new InvalidOperationException();
            }

            var taskContext = new ActorTaskContext(sector);
            return new MineTask(actor, taskContext, staticObject, toolMineDepositMethod);
        }

        private static Equipment? GetEquipedTool(IEquipmentModule equipmentModule, string[] requiredToolTags)
        {
            if (!requiredToolTags.Any())
            {
                // В этом методе предполагается, что наличие тегов проверено до вызова.
                throw new ArgumentException("Требуется не пустой набор тегов.", nameof(requiredToolTags));
            }

            foreach (var equipment in equipmentModule)
            {
                if (equipment is null)
                {
                    // Если для добычи указаны какие-либо теги, а ничего не экипировано,
                    // то такая экипировака не подходит.
                    continue;
                }

                if (equipment.Scheme.Tags is null)
                {
                    continue;
                }

                var equipmentTags = equipment.Scheme.Tags.Where(x => x != null).Select(x => x!).ToArray();

                var hasAllTags = EquipmentHelper.HasAllTags(equipmentTags, requiredToolTags);
                if (hasAllTags)
                {
                    // This equipment has all required tags.
                    return equipment;
                }
            }

            return null;
        }
    }
}