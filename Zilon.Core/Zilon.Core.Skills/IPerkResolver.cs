namespace Zilon.Core.Skills
{
    public interface IPerkResolver<TJobScheme> where TJobScheme : IMinimalJobSubScheme
    {
        /// <summary>
        /// Применяет прогресс к текущим работам.
        /// </summary>
        /// <param name="progress"> Объект прогресса. </param>
        /// <param name="evolutionData"> Данные о развитии персонажа. </param>
        /// <returns> Возвращает true, если все работы выполнены. </returns>
        void ApplyProgress(IJobProgress<TJobScheme> progress, ISkillManager skillManager);
    }
}