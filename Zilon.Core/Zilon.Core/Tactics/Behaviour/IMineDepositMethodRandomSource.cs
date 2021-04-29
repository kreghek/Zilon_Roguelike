using Zilon.Core.StaticObjectModules;

namespace Zilon.Core.Tactics.Behaviour
{
    /// <summary>
    /// Источник случайностей для методов добычи.
    /// </summary>
    public interface IMineDepositMethodRandomSource
    {
        /// <summary>
        /// Случайно выбирает усепшность добычи.
        /// </summary>
        /// <returns>true, если добыча прошла успешно. false - в ином случае.</returns>
        bool RollSuccess(DepositMiningDifficulty difficulty);
    }
}