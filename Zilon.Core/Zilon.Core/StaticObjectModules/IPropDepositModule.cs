namespace Zilon.Core.StaticObjectModules
{
    public interface IPropDepositModule : IStaticObjectModule
    {
        /// <summary>
        /// Признак того, что залежи исчерпаны.
        /// </summary>
        bool IsExhausted { get; }

        /// <summary>
        /// Сложность добычи ресурса из данного месторождения.
        /// </summary>
        DepositMiningDifficulty Difficulty { get; }

        /// <summary>
        /// Текущий запас. Когда равен 0 - залежи истощаются.
        /// </summary>
        float Stock { get; }

        /// <summary>
        /// Возвращает теги инструментов, которыми возможна разработка залежей.
        /// </summary>
        string[] GetToolTags();

        /// <summary>
        /// Выполняет добычу из залежей.
        /// </summary>
        void Mine();

        /// <summary>
        /// Выстреливает, когда происходит добыча.
        /// </summary>
        event EventHandler Mined;
    }
}