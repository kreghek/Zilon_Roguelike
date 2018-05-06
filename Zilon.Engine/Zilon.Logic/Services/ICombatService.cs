using Zilon.Logic.Tactics;
using Zilon.Logic.Tactics.Initialization;

namespace Zilon.Logic.Services
{
    public interface ICombatService
    {
        Combat CreateCombat(CombatInitData initData);
    }
}