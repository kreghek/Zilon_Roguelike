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
        /// Действие персонажа по умолчанию.
        /// </summary>
        public string DefaultAct { get; set; }

        /// <summary>
        /// Слоты экипировки.
        /// </summary>
        public PersonSlotSubScheme[] Slots { get; set; }
    }
}
