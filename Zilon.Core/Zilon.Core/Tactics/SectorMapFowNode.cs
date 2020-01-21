namespace Zilon.Core.Tactics
{
    /// <summary>
    /// Данные о тумане войны для одного узла карты сектора.
    /// </summary>
    public sealed class SectorMapFowNode
    {
        public SectorMapFowNode(OffsetCoords nodeCoords)
        {
            NodeCoords = nodeCoords;
        }

        public OffsetCoords NodeCoords { get; }

        public SectorMapNodeFowState State { get; private set; }

        public void ChangeState(SectorMapNodeFowState targetState)
        {
            State = targetState;
        }
    }
}
