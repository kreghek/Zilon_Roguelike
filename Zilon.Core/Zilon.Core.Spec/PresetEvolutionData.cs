using Zilon.Core.Persons;
using Zilon.Core.Schemes;

namespace Zilon.Core.Spec
{
    internal class PresetEvolutionData : EvolutionData
    {
        public PresetEvolutionData(ISchemeService schemeService, IPerk[] predefinedPerks) : base(schemeService)
        {
            Perks = predefinedPerks;
        }
    }
}
