namespace Zilon.Core.MapGenerators
{
    public interface IChestGeneratorRandomSource
    {
        int RollChestCount(int count);
        int RollNodeIndex(int nodeCount);
    }
}
