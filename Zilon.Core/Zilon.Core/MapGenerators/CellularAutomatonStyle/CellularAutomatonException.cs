namespace Zilon.Core.MapGenerators.CellularAutomatonStyle
{
    /// <summary>
    ///  Контроллируемое исключение, выбрасываемое при работе с клеточным автоматом.
    /// </summary>
    /// <remarks>
    /// Это исключение должно быть перехвачено как можно раньше и обработано.
    /// </remarks>
    [Serializable]
    public class CellularAutomatonException : Exception
    {
        /// <summary>
        /// Констрктор.
        /// </summary>
        public CellularAutomatonException() { }

        /// <summary>
        /// Констрктор.
        /// </summary>
        public CellularAutomatonException(string message) : base(message) { }

        /// <summary>
        /// Констрктор.
        /// </summary>
        public CellularAutomatonException(string message, Exception inner) : base(message, inner) { }

        /// <summary>
        /// Констрктор.
        /// </summary>
        protected CellularAutomatonException(
            System.Runtime.Serialization.SerializationInfo info,
            System.Runtime.Serialization.StreamingContext context) : base(info, context)
        {
        }
    }
}