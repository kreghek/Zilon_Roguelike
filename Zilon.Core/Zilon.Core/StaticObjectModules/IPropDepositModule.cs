using Zilon.Core.Schemes;

namespace Zilon.Core.StaticObjectModules
{
    [StaticObjectModule]
    public interface IPropDepositModule: IStaticObjectModule
    {
        /// <summary>
        /// Инструмент, необходимый для разработки залежей.
        /// </summary>
        IPropScheme Tool { get; }

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
