using Zilon.Core.Persons.Survival;

namespace Zilon.Core.Persons
{
    public interface ISurvivalRandomSource
    {
        int RollMaxHazardDamage();
        int RollSurvival(SurvivalStat stat);
    }
}