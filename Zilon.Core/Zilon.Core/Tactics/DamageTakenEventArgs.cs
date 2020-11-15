namespace Zilon.Core.Tactics
{
    public sealed class DamageTakenEventArgs : EventArgs
    {
        [ExcludeFromCodeCoverage]
        public DamageTakenEventArgs(int value)
        {
            if (value <= 0)
            {
                throw new InvalidOperationException();
            }

            Value = value;
        }

        /// <summary>
        /// Значение урона, который был нанесён.
        /// </summary>
        [PublicAPI]
        public int Value { get; }
    }
}