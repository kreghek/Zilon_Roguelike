namespace Zilon.Core.Tactics.Generation
{
    public interface ISectorGeneratorRandomSource
    {
        void RollRoomPosition(out int x, out int y);
        void RollRoomSize(out int w, out int h);
    }
}
