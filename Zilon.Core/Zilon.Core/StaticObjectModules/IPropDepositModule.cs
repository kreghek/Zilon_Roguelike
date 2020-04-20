using Zilon.Core.Schemes;

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
    }
}
