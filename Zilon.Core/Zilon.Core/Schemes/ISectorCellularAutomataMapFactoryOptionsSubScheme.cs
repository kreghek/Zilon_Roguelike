namespace Zilon.Core.Schemes
{
    public interface ISectorCellularAutomataMapFactoryOptionsSubScheme: ISectorMapFactoryOptionsSubScheme
    {
        int MapWidth { get; }

        int MapHeight { get; }

        int ChanceToStartAlive { get; }
    }
}
