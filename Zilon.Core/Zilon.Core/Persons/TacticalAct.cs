using Zilon.Core.Schemes;

namespace Zilon.Core.Persons
{
    public class TacticalAct : ITacticalAct
    {
        public TacticalAct(TacticalActScheme scheme)
        {
            Scheme = scheme;
            MinEfficient = scheme.Efficient.Min;
            MaxEfficient = scheme.Efficient.Max;
        }

        public TacticalActScheme Scheme { get; }

        public float MinEfficient { get; }

        public float MaxEfficient { get; }

    }
}
