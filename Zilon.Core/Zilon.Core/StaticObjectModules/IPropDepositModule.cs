namespace Zilon.Core.StaticObjectModules
{
    [StaticObjectModule]
    public interface IPropDepositModule: IStaticObjectModule
    {
        /// <summary>
        /// Возвращает теги инструментов, которыми возможна разработка залежей.
        /// </summary>
        string[] GetToolTags();

        /// <summary>
        /// Признак того, что залежи исчерпаны.
        /// </summary>
        bool IsExhausted { get; }

        /// <summary>
        /// Выполняет добычу из залежей.
        /// </summary>
        void Mine();

        /// <summary>
        /// Сложность добычи ресурса из данного месторождения.
        /// </summary>
        DepositMiningDifficulty Difficulty { get; }

        /// <summary>
        /// Текущий запас. Когда равен 0 - залежи истощаются.
        /// </summary>
        float Stock { get; }
    }
}
