using Zilon.Core.Persons;

namespace Zilon.Core.Tactics
{
    public interface ITacticalActUsageService
    {
        void UseOn(IActor actor, IAttackTarget target, ITacticalAct act);
    }
}
