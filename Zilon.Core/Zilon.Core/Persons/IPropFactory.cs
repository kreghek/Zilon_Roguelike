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

        /// <summary>
        ///  Создаёт ресурс на основе схемы.
        /// </summary>
        /// <param name="scheme"> Схема ресурса. </param>
        /// <returns></returns>
        Resource CreateResource(PropScheme scheme, int count);
    }
}