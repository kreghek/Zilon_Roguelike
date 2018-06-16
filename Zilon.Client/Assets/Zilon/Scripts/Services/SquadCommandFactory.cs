using System;
using Assets.Zilon.Scripts.Models.CombatScene;
using Zilon.Core.Commands;
using Zilon.Core.Services;
using Zilon.Core.Services.CombatEvents;

namespace Assets.Zilon.Scripts.Services
{
//    class SquadCommandFactory : ICommandFactory
//    {
//        private readonly IEventManager _eventManager;
//        private readonly ICombatPlayerState _combatPlayerState;
//        private readonly ICombatManager _combatManager;
//        //private readonly ICombatService _combatService;
//
//        public SquadCommandFactory(IEventManager eventManager, ICombatManager combatManager, ICombatPlayerState combatPlayerState/*, ICombatService combatService*/)
//        {
//            _eventManager = eventManager;
//            _combatPlayerState = combatPlayerState;
//            _combatManager = combatManager;
//            //_combatService = combatService;
//        }
//
//        ICommand ICommandFactory.CreateCommand<T>()
//        {
////            var command = Activator.CreateInstance(typeof(T),
////                _eventManager,
////                _combatManager,
////                _combatPlayerState,
////                _combatService);
////            return (ICommand)command;
//        }
//    }
}
