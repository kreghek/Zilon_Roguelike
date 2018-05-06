using System;
using Assets.Zilon.Scripts.Models.CombatScene;
using Assets.Zilon.Scripts.Models.Commands;
using UnityEngine;

namespace Assets.Zilon.Scripts.Services.CombatScene
{
    class PersonCommandHandler : IPersonCommandHandler
    {
        private readonly ICommandManager commandManager;
        private readonly ICombatManager combatManager;

        private CombatSquadVM selectedSquad;

        public PersonCommandHandler(ICommandManager commandManager, ICombatManager combatManager)
        {
            this.commandManager = commandManager;
            this.combatManager = combatManager;
        }

        public void LocationVM_OnSelect(object sender, EventArgs e)
        {
            if (selectedSquad != null)
            {
                var combat = combatManager.CurrentCombat;
                if (combat != null && commandManager != null)
                {
                    var moveCommand = new MoveCommand(combat, selectedSquad, sender as CombatLocationVM);
                    commandManager.Push(moveCommand);
                }
            }
        }

        public void SquadVM_OnSelect(object sender, EventArgs e)
        {
            selectedSquad = sender as CombatSquadVM;
            Debug.Log("selected " + selectedSquad);
        }
    }
}
