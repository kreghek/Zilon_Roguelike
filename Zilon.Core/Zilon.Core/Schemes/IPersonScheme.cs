namespace Zilon.Core.Schemes
{
    /// <summary>
    /// Схема персонажа.
    /// Это может быть персонаж игрока или напарник. Но не НПС и не монстр.
    /// </summary>
    public interface IPersonScheme : IScheme
    {
        string?[]? DefaultActs { get; set; }

        int Hp { get; set; }

        PersonSlotSubScheme?[]? Slots { get; set; }

        /// <summary>
        /// Характеристики выживания персонажа.
        /// Такие, как запас сытости/гидратации.
        /// </summary>
        IPersonSurvivalStatSubScheme?[]? SurvivalStats { get; }
    }
}