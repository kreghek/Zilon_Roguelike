using Zilon.Core.Tactics;

namespace Zilon.Core.MapGenerators
{
    public interface ISectorProceduralGenerator
    {
        ISector Generate();
    }
}