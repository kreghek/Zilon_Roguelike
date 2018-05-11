using System;
using Assets.Zilon.Scripts.Models.CombatScene;
using Zilon.Core.Commands;

namespace Assets.Zilon.Scripts.Services
{
    class SquadCommandFactory : ICommandFactory
    {
        private ICombatPlayerState _combatPlayerState;

        public SquadCommandFactory(ICombatPlayerState combatPlayerState)
        {
            _combatPlayerState = combatPlayerState;
        }

        ICommand ICommandFactory.CreateCommand<T>()
        {
            var command = Activator.CreateInstance(typeof(T), _combatPlayerState);
            return (ICommand)command;
        }
    }
}
