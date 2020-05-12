using Zilon.Core.Tactics;
using Zilon.Core.World;

namespace Zilon.Core.MapGenerators
{
    /// <summary>
    /// Источник рандома для генератора статических объектов.
    /// </summary>
    public interface IStaticObjectsGeneratorRandomSource
    {
        /// <summary>
        /// Выбрать случайное назначение статического объекта.
        /// </summary>
        /// <param name="purposes"> Доступные назначения. </param>
        /// <returns> Возвращает выбранное назначение. </returns>
        PropContainerPurpose RollPurpose(IResourceDepositData resourceDepositData);
    }
}
