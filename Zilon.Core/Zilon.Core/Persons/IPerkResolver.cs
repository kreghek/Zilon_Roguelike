namespace Zilon.Core.Persons
{
    public interface IPerkResolver
    {
        /// <summary>
        /// Применяет прогресс к текущим работам.
        /// </summary>
        /// <param name="progress"> Объект прогресса. </param>
        /// <param name="executable"> Объект, которые выполняется при помощи работ. </param>
        /// <returns> Возвращает true, если все работы выполнены. </returns>
        bool ApplyProgress(IJobProgress progress, IJobExecutable executable);
    }
}
