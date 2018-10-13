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
        /// <returns> Возвращает экземпляр созданной экипировки. </returns>
        Equipment CreateEquipment(IPropScheme scheme);

        /// <summary>
        ///  Создаёт ресурс на основе схемы.
        /// </summary>
        /// <param name="scheme"> Схема ресурса. </param>
        /// <param name="count"> Количество ресурса. </param>
        /// <returns> Возвращает экземпляр созданного ресурса. </returns>
        Resource CreateResource(IPropScheme scheme, int count);
    }
}