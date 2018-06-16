using System.Linq;
using Assets.Zilon.Scripts.Models.CombatScene;
using UnityEngine;
using Zilon.Core.Services;
using Zilon.Core.Services.CombatEvents;
using Zilon.Core.Tactics.Events;

namespace Assets.Zilon.Scripts.Models.Commands
{
    /// <summary>
    /// Команда на завершение хода текущим взводом.
    /// </summary>
//    class EndTurnCommand : SquadCommandBase
//    {
//
//        public EndTurnCommand(IEventManager eventManager,
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
//
//
//            var tacticCommandResult = _combatService.EndTurnCommand(combat);
//            var tacticCommandCompleted = tacticCommandResult.Type == CommandResultType.Complete;
//            var hasNoCommandErrors = tacticCommandResult.Errors?.Any();
//
//            Debug.Log(tacticCommandResult.Events);
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