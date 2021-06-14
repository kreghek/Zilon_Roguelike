namespace Zilon.Core.Tactics
{
    public interface ITacticalActUsageService
    {
        void UseOn(IActor actor, ActTargetInfo target, UsedTacticalActs usedActs, ISector sector);
    }
}