namespace Zilon.Core.Schemes
{
    /// <summary>
    /// Monster schematic.
    /// </summary>
    /// <seealso cref="IScheme" />
    public interface IMonsterScheme : IScheme
    {
        /// <summary>
        /// Base points awarded for killing a monster.
        /// </summary>
        int BaseScore { get; }

        /// <summary>
        /// The action the monster performs when attacking.
        /// </summary>
        ITacticalActStatsSubScheme?[]? CombatActs { get; }

        /// <summary>
        /// The characteristics of the monster's defense.
        /// </summary>
        IMonsterDefenseSubScheme? Defense { get; }

        /// <summary>
        /// Tables of drop, from which the loot from the monster is formed.
        /// </summary>
        string?[]? DropTableSids { get; }

        /// <summary>
        /// The HP value of the monster.
        /// </summary>
        int Hp { get; }

        /// <summary>
        /// The coefficient of the speed of movement of the monster.
        /// </summary>
        /// <remarks>
        /// The higher the value, the faster the monster.
        /// With a coefficient of 2, the monster manages to move twice in one iteration of the sector.
        /// </remarks>
        float? MoveSpeedFactor { get; }

        /// <summary>
        /// Tags for classifying monsters.
        /// </summary>
        string?[]? Tags { get; }
    }
}