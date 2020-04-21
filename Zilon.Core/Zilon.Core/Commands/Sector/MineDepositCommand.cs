using System;
using System.Linq;

using Zilon.Core.Client;
using Zilon.Core.Persons;
using Zilon.Core.Props;
using Zilon.Core.StaticObjectModules;
using Zilon.Core.Tactics;
using Zilon.Core.Tactics.Behaviour;

namespace Zilon.Core.Commands.Sector
{
    public sealed class MineDepositCommand : ActorCommandBase
    {
        private readonly IMineDepositMethodRandomSource _mineDepositMethodRandomSource;

        public MineDepositCommand(
            IGameLoop gameLoop,
            ISectorManager sectorManager,
            ISectorUiState playerState,
            IMineDepositMethodRandomSource mineDepositMethodRandomSource) : base(gameLoop, sectorManager, playerState)
        {
            _mineDepositMethodRandomSource = mineDepositMethodRandomSource;
        }

        public override bool CanExecute()
        {
            var selectedViewModel = PlayerState.SelectedViewModel ?? PlayerState.HoverViewModel;
            var targetDeposit = (selectedViewModel as IContainerViewModel)?.Container.GetModuleSafe<IPropDepositModule>();

            if (targetDeposit is null)
            {
                return false;
            }

            var equipmentCarrier = PlayerState.ActiveActor.Actor.Person.EquipmentCarrier;
            var requiredTags = targetDeposit.GetToolTags();
            if (requiredTags.Any())
            {
                var equipedTool = GetEquipedTool(equipmentCarrier, targetDeposit.GetToolTags());
                if (equipedTool is null)
                {
                    return false;
                }

                return true;
            }
            else
            {
                // Если для добычи не указаны теги, то предполагается,
                // что добывать можно "руками".
                // То есть никакого инструмента не требуется.
                return true;
            }
        }

        protected override void ExecuteTacticCommand()
        {
            var targetStaticObject = (PlayerState.SelectedViewModel as IContainerViewModel).Container;
            var targetDeposit = targetStaticObject.GetModule<IPropDepositModule>();

            var equipmentCarrier = PlayerState.ActiveActor.Actor.Person.EquipmentCarrier;
            var requiredTags = targetDeposit.GetToolTags();

            if (requiredTags.Any())
            {
                var equipedTool = GetEquipedTool(equipmentCarrier, requiredTags);
                if (equipedTool is null)
                {
                    throw new InvalidOperationException("Попытка добычи без инструмента.");
                }
                else
                {
                    var intetion = new Intention<MineTask>(actor => CreateTaskByInstrument(actor, targetStaticObject, equipedTool));
                    PlayerState.TaskSource.Intent(intetion);
                }
            }
            else
            {
                // Добыча руками, если никаких тегов инструмента не задано.
                var intetion = new Intention<MineTask>(actor => CreateTaskByHands(actor, targetStaticObject));
                PlayerState.TaskSource.Intent(intetion);
            }
        }

        private static Equipment GetEquipedTool(IEquipmentCarrier equipmentCarrier, string[] requiredToolTags)
        {
            if (!requiredToolTags.Any())
            {
                // В этом методе предполагается, что наличие тегов проверено до вызова.
                throw new ArgumentException("Требуется не пустой набор тегов.", nameof(requiredToolTags));
            }

            foreach (var equipment in equipmentCarrier)
            {
                if (equipment is null)
                {
                    // Если для добычи указаны какие-либо теги, а ничего не экипировано,
                    // то такая экипировака не подходит.
                    continue;
                }

                if (equipment.Scheme.Tags.Intersect(requiredToolTags) == requiredToolTags)
                {
                    // У экипировки должны быть все требуемые теги.
                    return equipment;
                }
            }

            return null;
        }

        private MineTask CreateTaskByInstrument(IActor actor, IStaticObject staticObject, Equipment equipedTool)
        {
            var toolMineDepositMethod = new ToolMineDepositMethod(equipedTool, _mineDepositMethodRandomSource);
            var map = SectorManager.CurrentSector.Map;
            return new MineTask(actor, staticObject, toolMineDepositMethod, map);
        }

        private MineTask CreateTaskByHands(IActor actor, IStaticObject staticObject)
        {
            var handMineDepositMethod = new HandMineDepositMethod(_mineDepositMethodRandomSource);
            var map = SectorManager.CurrentSector.Map;
            return new MineTask(actor, staticObject, handMineDepositMethod, map);
        }
    }
}
