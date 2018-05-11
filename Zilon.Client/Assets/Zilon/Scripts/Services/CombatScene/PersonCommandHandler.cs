using System;
using Assets.Zilon.Scripts.Models.CombatScene;
using Assets.Zilon.Scripts.Models.Commands;
using UnityEngine;
using Zenject;
using Zilon.Core.Commands;
using Zilon.Core.Services;
using Zilon.Core.Services.CombatEvents;

namespace Assets.Zilon.Scripts.Services.CombatScene
{
    class PersonCommandHandler : IPersonCommandHandler
    {
        private readonly ICommandManager commandManager;
        private readonly ICombatManager combatManager;
        private readonly ICombatService combatService;
        private readonly IEventManager eventManager;
        private readonly ICommandFactory _commandFactory;
        private readonly ICombatPlayerState _combatPlayerState;

        public PersonCommandHandler(IEventManager eventManager,
            ICommandManager commandManager, 
            ICombatManager combatManager,
            ICombatService combatService,
            [Inject(Id = "squad-command-factory")] ICommandFactory commandFactory,
            ICombatPlayerState combatPlayerState)
        {
            this.commandManager = commandManager;
            this.combatManager = combatManager;
            this.combatService = combatService;
            this.eventManager = eventManager;
            _commandFactory = commandFactory;
            _combatPlayerState = combatPlayerState;
        }

        public void LocationVM_OnSelect(object sender, EventArgs e)
        {
            if (_combatPlayerState.SelectedSquad != null)
            {
                var selectedNode = sender as CombatLocationVM;
                Debug.Log($"Selected {selectedNode}");
                _combatPlayerState.SelectedNode = selectedNode;
                var moveCommand = _commandFactory.CreateCommand<MoveCommand>();
                commandManager.Push(moveCommand);
            }
        }

        public void SquadVM_OnSelect(object sender, EventArgs e)
        {
            _combatPlayerState.SelectedSquad = sender as CombatSquadVM;
            Debug.Log("selected " + _combatPlayerState.SelectedSquad);
        }
    }
}
