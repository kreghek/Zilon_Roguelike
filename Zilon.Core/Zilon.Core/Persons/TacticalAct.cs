using Zilon.Core.Schemes;

namespace Zilon.Core.Persons
{
    public class TacticalAct : ITacticalAct
    {
        public TacticalAct(float power, TacticalActScheme scheme)
        {
            Scheme = scheme;
            MinEfficient = scheme.Efficient.Min * power;
            MaxEfficient = scheme.Efficient.Max * power;
        }

        public TacticalActScheme Scheme { get; }

        public float MinEfficient { get; }

        public float MaxEfficient { get; }

    }
}
