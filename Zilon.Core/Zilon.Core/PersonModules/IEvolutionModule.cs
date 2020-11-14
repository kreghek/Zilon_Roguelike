using Zilon.Core.Components;
using Zilon.Core.Persons;

namespace Zilon.Core.PersonModules
{
    /// <summary>
    ///     Интерфейс, содержащий данные о развитие персонажа.
    /// </summary>
    public interface IEvolutionModule : IPersonModule
    {
        /// <summary>
        ///     Перечень навыков.
        /// </summary>
        SkillStatItem[] Stats { get; }

        /// <summary>
        ///     Текущие перки.
        /// </summary>
        IPerk[] Perks { get; }

        /// <summary>
        ///     Указывает, что один из активных перков считается прокачанным.
        /// </summary>
        /// <param name="perk"> Активный перк, который следует считать достигнутым. </param>
        void PerkLevelUp(IPerk perk);

        /// <summary>
        ///     Добавить врождённые перки.
        /// </summary>
        /// <param name="perks"> Набор врождённых перков. </param>
        /// <remarks>
        ///     Метод нужен, потому что реализация этого интерфейса сама собирает данные о доступных перках.
        ///     И реализации нужно знать, какие перки врожденные, а какие приобретаются по мере развития персонажа.
        /// </remarks>
        void AddBuildInPerks(IEnumerable<IPerk> perks);

        /// <summary>
        ///     Выстреливает, когда один из перков повышается на уровень.
        /// </summary>
        event EventHandler<PerkEventArgs> PerkLeveledUp;

        /// <summary>
        ///     Выстреливает, когда появляется новый перк.
        /// </summary>
        event EventHandler<PerkEventArgs> PerkAdded;
    }
}