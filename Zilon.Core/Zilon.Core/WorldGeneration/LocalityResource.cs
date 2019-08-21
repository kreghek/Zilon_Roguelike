namespace Zilon.Core.WorldGeneration
{
    /// <summary>
    /// Ресурсы города.
    /// </summary>
    public enum LocalityResource
    {
        /// <summary>
        /// Не определено. Скорее всего, ошибка.
        /// </summary>
        Undefined,

        /// <summary>
        /// Жилые места для населения.
        /// </summary>
        LivingPlaces,

        /// <summary>
        /// Еда.
        /// </summary>
        Food,

        /// <summary>
        /// Энергия.
        /// </summary>
        Energy,

        /// <summary>
        /// Товары народного потребления (не еда).
        /// </summary>
        Goods,

        /// <summary>
        /// Производственные товары.
        /// </summary>
        Manufacture,
    }
}
