using Zilon.Core.Schemes;

namespace Zilon.Core.Persons
{
    /// <summary>
    /// Фабрика предметов.
    /// </summary>
    public interface IPropFactory
    {
        /// <summary>
        ///  Создаёт экипировку на основе схемы.
        /// </summary>
        /// <param name="scheme"> Схема экипировки. </param>
        /// <returns></returns>
        Equipment CreateEquipment(PropScheme scheme);
    }
}