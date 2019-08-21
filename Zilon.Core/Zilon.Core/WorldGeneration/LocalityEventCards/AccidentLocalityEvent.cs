using Zilon.Core.CommonServices.Dices;

namespace Zilon.Core.WorldGeneration.LocalityEventCards
{
    /// <summary>
    /// В городе произошла черезвычайная ситуация.
    /// Например, взорвался завод и экология сильно пострадала.
    /// </summary>
    public sealed class AccidentLocalityEvent : ILocalityEventCard
    {
        public bool CanUse(Locality locality, Globe globe)
        {
            return true;
        }

        public void Use(Locality locality, Globe globe, IDice dice)
        {
            locality.Population--;

            // Возможно, следует добавить случайный урон по деятелям в городе.
        }
    }
}
