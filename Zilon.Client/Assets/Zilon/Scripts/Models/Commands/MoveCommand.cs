using System.Linq;
using Assets.Zilon.Scripts.Models.CombatScene;
using UnityEngine;
using Zilon.Core.Services;
using Zilon.Core.Services.CombatEvents;
using Zilon.Core.Tactics.Events;

namespace Assets.Zilon.Scripts.Models.Commands
{
    /// <summary>
    /// Команда на перемещение взвода в указанный узел карты.
    /// </summary>
//    class MoveCommand : SquadCommandBase
//    {
//
//        public MoveCommand(IEventManager eventManager,
//            ICombatManager combatManager,
//            ICombatPlayerState combatPlayerState,
//            ICombatService combatService) : 
//            base(eventManager, combatManager, combatPlayerState, combatService)
//        {
//            
//        }
//
//        public override bool CanExecute()
//        {
//            return true;
//        }
//
//        protected override ITacticEvent[] ExecuteTacticCommand()
//        {
//            var combat = _combatManager.CurrentCombat;
//            var selectedSquadVM = _combatPlayerState.SelectedSquad;
//            var selectedNodeVM = _combatPlayerState.SelectedNode;
//
//
//            var tacticCommandResult = _combatService.MoveCommand(combat, selectedSquadVM.ActorSquad, selectedNodeVM.Node);
//            var tacticCommandCompleted = tacticCommandResult.Type == CommandResultType.Complete;
//            var hasNoCommandErrors = tacticCommandResult.Errors?.Any();
//
//            if (hasNoCommandErrors.GetValueOrDefault())
//            {
//                Debug.Log($"MoveCommand events: {tacticCommandResult.Events}");
//            }
//            else
//            {
//                Debug.Log($"MoveCommand result: {tacticCommandResult.Type}");
//                if (tacticCommandResult.Errors != null)
//                {
//                    Debug.Log($"MoveCommand errors: {string.Join(", ", tacticCommandResult.Errors)}");
//                }
//            }
//
//            if (!tacticCommandCompleted || hasNoCommandErrors.GetValueOrDefault())
//            {
//                return null;
//            }
//
//            return tacticCommandResult.Events;
//        }
//    }
}