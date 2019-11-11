using Zilon.Core.CommonServices.Dices;

namespace Zilon.Core.WorldGeneration
{
    /// <summary>
    /// Карта события города.
    /// </summary>
    public interface ILocalityEventCard
    {
        bool CanUse(Locality locality, Globe globe);
        void Use(Locality locality, Globe globe, IDice dice);
    }
}
