using Zilon.Core.StaticObjectModules;

namespace Zilon.Core.Tactics.Behaviour
{
    /// <summary>
    ///     Метод открытия контейнера.
    ///     Может быть просто открытие, открытие ключом, открытие отмычкой.
    /// </summary>
    public interface IOpenContainerMethod
    {
        /// <summary>
        ///     Попытка открыть контейнер.
        /// </summary>
        /// <param name="container"> Целевой контейнер. </param>
        /// <returns> Возвращает результат вскрытия контейнера. </returns>
        IOpenContainerResult TryOpen(IPropContainer container);
    }
}