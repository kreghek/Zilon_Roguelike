using Zilon.Core.Persons.Survival;

namespace Zilon.Core.Persons
{
    public interface ISurvivalRandomSource
    {
        int RollSurvival(SurvivalStat stat);

        int RollMaxHazardDamage();
    }
}