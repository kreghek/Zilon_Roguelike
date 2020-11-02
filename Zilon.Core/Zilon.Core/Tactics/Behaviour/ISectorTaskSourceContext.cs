using Zilon.Core.World;

namespace Zilon.Core.Tactics.Behaviour
{
    public interface ISectorTaskSourceContext
    {
        ISector Sector { get; }

        IGlobe Globe { get; }
    }
}