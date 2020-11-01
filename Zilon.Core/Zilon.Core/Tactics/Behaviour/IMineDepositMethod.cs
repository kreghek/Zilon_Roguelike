using Zilon.Core.StaticObjectModules;

namespace Zilon.Core.Tactics.Behaviour
{
    /// <summary>
    ///     Метод добычи ресурса из залежей.
    ///     Может быть руками или инструментом.
    /// </summary>
    public interface IMineDepositMethod
    {
        /// <summary>
        ///     Попытка добыть ресурс из залежей.
        /// </summary>
        /// <param name="container"> Целевые залежи. </param>
        /// <returns> Возвращает результат добычи. </returns>
        IMineDepositResult TryMine(IPropDepositModule deposit);
    }
}