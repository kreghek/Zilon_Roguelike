using Assets.Zilon.Scripts.Models.CombatScene;
using Zilon.Core.Commands;
using Zilon.Core.Services;
using Zilon.Core.Services.CombatEvents;

namespace Assets.Zilon.Scripts.Models.Commands
{
    /// <summary>
    /// Базовая команда для всех команд, связанных с изменением состояния взвода.
    /// </summary>
    abstract class SquadCommandBase : TacticCommandBase
    {
        protected readonly ICombatPlayerState _combatPlayerState;
        protected readonly ICombatManager _combatManager;
        //protected readonly ICombatService _combatService;

        public SquadCommandBase(IEventManager eventManager, ICombatManager combatManager, ICombatPlayerState combatPlayerState/*, ICombatService combatService*/): base(eventManager)
        {
            _combatPlayerState = combatPlayerState;
            _combatManager = combatManager;
            //_combatService = combatService;
        }
    }
}
