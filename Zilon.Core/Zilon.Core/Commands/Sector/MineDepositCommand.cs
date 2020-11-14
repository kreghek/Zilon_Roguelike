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

        public override bool CanExecute()
        {
            ISelectableViewModel selectedViewModel = PlayerState.SelectedViewModel ?? PlayerState.HoverViewModel;
            IPropDepositModule targetDeposit = (selectedViewModel as IContainerViewModel)?.StaticObject
                .GetModuleSafe<IPropDepositModule>();

            if (targetDeposit is null)
            {
                return false;
            }

            IEquipmentModule equipmentCarrier = PlayerState.ActiveActor.Actor.Person.GetModuleSafe<IEquipmentModule>();
            if (equipmentCarrier is null)
            {
                return false;
            }

            var requiredTags = targetDeposit.GetToolTags();
            if (requiredTags.Any())
            {
                Equipment equipedTool = GetEquipedTool(equipmentCarrier, targetDeposit.GetToolTags());
                if (equipedTool is null)
                {
                    return false;
                }

                return true;
            }

            // Если для добычи не указаны теги, то предполагается,
            // что добывать можно "руками".
            // То есть никакого инструмента не требуется.
            return true;
        }

        protected override void ExecuteTacticCommand()
        {
            IStaticObject targetStaticObject = (PlayerState.SelectedViewModel as IContainerViewModel).StaticObject;
            IPropDepositModule targetDeposit = targetStaticObject.GetModule<IPropDepositModule>();

            IEquipmentModule equipmentCarrier = PlayerState.ActiveActor.Actor.Person.GetModule<IEquipmentModule>();
            var requiredTags = targetDeposit.GetToolTags();

            if (requiredTags.Any())
            {
                Equipment equipedTool = GetEquipedTool(equipmentCarrier, requiredTags);
                if (equipedTool is null)
                {
                    throw new InvalidOperationException("Попытка добычи без инструмента.");
                }

                Intention<MineTask> intetion = new Intention<MineTask>(actor =>
                    CreateTaskByInstrument(actor, targetStaticObject, equipedTool));
                PlayerState.TaskSource.Intent(intetion, PlayerState.ActiveActor.Actor);
            }
            else
            {
                // Добыча руками, если никаких тегов инструмента не задано.
                Intention<MineTask> intetion =
                    new Intention<MineTask>(actor => CreateTaskByHands(actor, targetStaticObject));
                PlayerState.TaskSource.Intent(intetion, PlayerState.ActiveActor.Actor);
            }
        }

        private static Equipment GetEquipedTool(IEquipmentModule equipmentModule, string[] requiredToolTags)
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

                var hasAllTags = EquipmentHelper.HasAllTags(equipment.Scheme.Tags, requiredToolTags);
                if (hasAllTags)
                {
                    // This equipment has all required tags.
                    return equipment;
                }
            }

            return null;
        }

        private MineTask CreateTaskByInstrument(IActor actor, IStaticObject staticObject, Equipment equipedTool)
        {
            ToolMineDepositMethod toolMineDepositMethod =
                new ToolMineDepositMethod(equipedTool, _mineDepositMethodRandomSource);

            ActorTaskContext taskContext = new ActorTaskContext(_player.SectorNode.Sector);
            return new MineTask(actor, taskContext, staticObject, toolMineDepositMethod);
        }

        private MineTask CreateTaskByHands(IActor actor, IStaticObject staticObject)
        {
            HandMineDepositMethod handMineDepositMethod = new HandMineDepositMethod(_mineDepositMethodRandomSource);

            ActorTaskContext taskContext = new ActorTaskContext(_player.SectorNode.Sector);
            return new MineTask(actor, taskContext, staticObject, handMineDepositMethod);
        }
    }
}