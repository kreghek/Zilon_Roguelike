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
        [UsedImplicitly]
        public string DefaultAct { get; set; }

        /// <summary>
        /// Слоты экипировки.
        /// </summary>
        [UsedImplicitly]
        public PersonSlotSubScheme[] Slots { get; set; }

        /// <summary>
        /// Характеристики выживания персонажа.
        /// Такие, как запас сытости/гидратации.
        /// </summary>
        [JsonConverter(typeof(ConcreteTypeConverter<PersonSurvivalStatSubScheme[]>))]
        [JsonProperty]
        [UsedImplicitly]
        public IPersonSurvivalStatSubScheme[] SurvivalStats { get; private set; }
    }
}