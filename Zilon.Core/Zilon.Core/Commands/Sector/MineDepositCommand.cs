using System;
using Zilon.Core.Client;
using Zilon.Core.Props;
using Zilon.Core.Schemes;
using Zilon.Core.StaticObjectModules;
using Zilon.Core.Tactics;
using Zilon.Core.Tactics.Behaviour;

namespace Zilon.Core.Commands.Sector
{
    public class MineDepositCommand : ActorCommandBase
    {
        public MineDepositCommand(
            IGameLoop gameLoop,
            ISectorManager sectorManager,
            ISectorUiState playerState) : base(gameLoop, sectorManager, playerState)
        {
        }

        public override bool CanExecute()
        {
            var targetDeposit = (PlayerState.HoverViewModel as IContainerViewModel)?.Container.GetModuleSafe<IPropDepositModule>();

            if (targetDeposit is null)
            {
                return false;
            }

            var equipedTool = GetEquipedTool(targetDeposit.Tool);
            if (equipedTool is null)
            {
                return false;
            }

            return true;
        }

        protected override void ExecuteTacticCommand()
        {
            var targetStaticObject = (PlayerState.SelectedViewModel as IContainerViewModel).Container;
            var targetDeposit = targetStaticObject.GetModule<IPropDepositModule>();

            var equipedTool = GetEquipedTool(targetDeposit.Tool);
            if (equipedTool is null)
            {
                // Потенциально здесь вместо исключения можно генерировать методы добычи руками.
                throw new InvalidOperationException("Попытка добычи без инструмента");
            }
            else
            {
                var intetion = new Intention<MineTask>(actor => CreateTask(actor, targetStaticObject, equipedTool));
                PlayerState.TaskSource.Intent(intetion);
            }
        }

        private Equipment GetEquipedTool(IPropScheme requiredToolScheme)
        {
            foreach (var equipment in PlayerState.ActiveActor.Actor.Person.EquipmentCarrier)
            {
                if (equipment is null)
                {
                    continue;
                }

                if (equipment.Scheme == requiredToolScheme)
                {
                    return equipment;
                }
            }

            return null;
        }

        private MineTask CreateTask(IActor actor, IStaticObject staticObject, Equipment equipedTool)
        {
            var toolMineDepositMethod = new ToolMineDepositMethod(equipedTool);
            return new MineTask(actor, staticObject, toolMineDepositMethod, SectorManager.CurrentSector.Map);
        }
    }
}
