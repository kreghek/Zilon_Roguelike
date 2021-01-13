namespace Zilon.Core.Scoring
{
    /// <summary>
    /// The event used to generate fake death reason when the iteration limet reached in the tests and benchs.
    /// It required to avoid error with no death reason at all.
    /// </summary>
    public sealed class EndOfLifeEvent : IPlayerEvent
    {
        public string Key => "the-end";
        public int Weight => 1;
    }
}