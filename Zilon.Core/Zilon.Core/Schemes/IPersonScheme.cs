using JetBrains.Annotations;

namespace Zilon.Core.Schemes
{
    /// <summary>
    /// Схема персонажа.
    /// Это может быть персонаж игрока или напарник. Но не НПС и не монстр.
    /// </summary>
    public interface IPersonScheme : IScheme
    {
        [NotNull] string DefaultAct { get; set; }

        int Hp { get; set; }

        [NotNull] [ItemNotNull] PersonSlotSubScheme[] Slots { get; set; }

        /// <summary>
        /// Характеристики выживания персонажа.
        /// Такие, как запас сытости/гидратации.
        /// </summary>
        [NotNull]
        IPersonSurvivalStatSubScheme[] SurvivalStats { get; }
    }
}