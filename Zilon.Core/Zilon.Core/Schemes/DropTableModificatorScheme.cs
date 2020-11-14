namespace Zilon.Core.Schemes
{
    /// <summary>
    ///     Модификатор таблицы дропа.
    /// </summary>
    public sealed class DropTableModificatorScheme : SchemeBase, IDropTableModificatorScheme
    {
        /// <summary>
        ///     Идентификаторы схем предметов, на которые модификатор влияет.
        /// </summary>
        public string[] PropSids { get; set; }

        /// <summary>
        ///     Множитель от 0 до 1, на сколько увеличивается/уменьшается вес указанных предметов.
        /// </summary>
        public float WeightBonus { get; set; }
    }
}