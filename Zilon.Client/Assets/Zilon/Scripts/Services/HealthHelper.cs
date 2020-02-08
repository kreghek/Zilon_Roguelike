using Zilon.Core.Persons.Survival;

namespace Assets.Zilon.Scripts.Services
{
    static class HealthHelper
    {
        private const float HEALTHLY_STATE = 0.95f;
        private const float SLIGHTLY_INJURED_STATE = 0.75f;
        private const float WOUNDED_STATE = 0.5f;
        private const float AT_DEATH_STATE = 0.25f;

        public static string GetHealthStateKey(SurvivalStat hpStat)
        {
            var hpPercentage = hpStat.ValueShare;

            if (hpPercentage >= HEALTHLY_STATE)
            {
                return "healthy";
            }
            if (SLIGHTLY_INJURED_STATE <= hpPercentage && hpPercentage < HEALTHLY_STATE)
            {
                return "slightly-injured";
            }
            else if (WOUNDED_STATE <= hpPercentage && hpPercentage < SLIGHTLY_INJURED_STATE)
            {
                return "wounded";
            }
            else if (AT_DEATH_STATE <= hpPercentage && hpPercentage < WOUNDED_STATE)
            {
                return "badly-wounded";
            }
            else
            {
                return "at-death";
            }
        }
    }
}
