namespace Zilon.Core.Scoring
{
    public sealed class EndOfLifeEvent : IPlayerEvent
    {
        public string Key => "the-end";
        public int Weight => 1;
    }
}