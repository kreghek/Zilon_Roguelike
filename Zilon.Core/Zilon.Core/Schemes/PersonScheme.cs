namespace Zilon.Core.Schemes
{
    /// <summary>
    /// Схема персонажа.
    /// </summary>
    public class PersonScheme : SchemeBase, IPersonScheme
    {
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
