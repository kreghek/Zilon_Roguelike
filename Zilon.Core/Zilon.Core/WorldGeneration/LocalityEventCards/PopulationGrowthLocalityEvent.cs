using Zilon.Core.CommonServices.Dices;

namespace Zilon.Core.WorldGeneration.LocalityEventCards
{
    public sealed class PopulationGrowthLocalityEvent : ILocalityEventCard
    {
        public bool CanUse(Locality locality, Globe globe)
        {
            return true;
        }

        public void Use(Locality locality, Globe globe, IDice dice)
        {
            locality.Population++;

            // Возможно, следует добавить вероятность появления нового деятеля.
        }
    }
}
