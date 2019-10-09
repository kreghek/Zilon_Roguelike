namespace Zilon.Core.MapGenerators.CellularAutomatonStyle
{
    public interface IInteriorObjectRandomSource
    {
        InteriorObjectMeta[] RollInteriorObjects(OffsetCoords[] regionDraftCoords);
    }
}
