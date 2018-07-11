namespace Zilon.Core.Schemes
{
    /// <inheritdoc />
    /// <summary>
    /// Схема персонажа.
    /// </summary>
    public class PersonScheme: SchemeBase
    {
        //TODO Убрать из схем сеттеры
        /// <summary>
        /// Базовые хитпоинты персонажа.
        /// </summary>
        public int Hp { get; set; }

        /// <summary>
        /// Количество слотов в экипировке.
        /// </summary>
        public int SlotCount { get; set; }
    }
}
