namespace Zilon.Core.Schemes
{
    /// <summary>
    /// Схема монстра.
    /// </summary>
    /// <seealso cref="Zilon.Core.Schemes.IScheme" />
    public interface IMonsterScheme: IScheme
    {
        /// <summary>
        /// Характеристики обороны монстра.
        /// </summary>
        IMonsterDefenseSubScheme Defense { get; }

        /// <summary>
        /// Таблицы дропа, из которых формируется лут с монстра.
        /// </summary>
        string[] DropTableSids { get; }

        /// <summary>
        /// Значение ХП монстра.
        /// </summary>
        int Hp { get; }

        /// <summary>
        /// Действие, которое монстр выполняет при атаке.
        /// </summary>
        ITacticalActStatsSubScheme PrimaryAct { get; }

        /// <summary>
        /// Базовые очки, начисляемые за убиство монстра.
        /// </summary>
        int BaseScore { get; }
    }
}