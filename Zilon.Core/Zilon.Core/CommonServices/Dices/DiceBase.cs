namespace Zilon.Core.CommonServices.Dices
{
    /// <summary>
    ///     Базовая кость для всех костей.
    /// </summary>
    public abstract class DiceBase
    {
        private readonly Random _random;

        /// <summary>
        ///     Конструктор генератора.
        /// </summary>
        [ExcludeFromCodeCoverage]
        protected DiceBase()
        {
            _random = new Random();
        }

        /// <summary>
        ///     Конструктор кости.
        /// </summary>
        /// <param name="seed"> Зерно рандомизации. </param>
        /// <remarks>
        ///     При одном и том же зерне рандомизации будет генерироваться
        ///     одна и та же последовательность случайных чисел.
        /// </remarks>
        [ExcludeFromCodeCoverage]
        protected DiceBase(int seed)
        {
            _random = new Random(seed);
        }

        /// <summary>
        ///     Возвращает случайное число от [0..1).
        /// </summary>
        protected double GetNextDouble()
        {
            var next = _random.NextDouble();
            return next;
        }
    }
}