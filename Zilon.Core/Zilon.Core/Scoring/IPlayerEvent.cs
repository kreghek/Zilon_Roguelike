namespace Zilon.Core.Scoring
{
    public interface IPlayerEvent
    {
        string Key { get; }

        int Weight { get; }
    }
}
