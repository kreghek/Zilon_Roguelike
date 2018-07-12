namespace Zilon.Core.Tactics.Behaviour
{
    /// <summary>
    /// Метод открытия контейнера.
    /// Может быть просто открытие, открытие ключом, открытие отмычкой.
    /// </summary>
    public interface IOpenContainerMethod
    {
        /// <summary>
        /// Попытка открыть контейнер.
        /// </summary>
        /// <param name="container"> Целевой контейнер. </param>
        /// <returns> Возвращает true, если контейнер успешно открыт. Иначе false. </returns>
        bool TryOpen(IPropContainer container);
    }
}
