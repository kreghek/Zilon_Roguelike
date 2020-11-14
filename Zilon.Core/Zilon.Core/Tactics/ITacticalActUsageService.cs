namespace Zilon.Core.Tactics
{
    public interface ITacticalActUsageService
    {
        void UseOn(
            IActor actor,
            IAttackTarget target,
            UsedTacticalActs usedActs,
            ISector sector);
    }
}