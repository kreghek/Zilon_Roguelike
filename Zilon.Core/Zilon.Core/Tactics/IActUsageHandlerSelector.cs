namespace Zilon.Core.Tactics
{
    /// <summary>
    /// Сервис для аккумуляции обработчиков применения дейсвий к цели и выбора обработчика по цели.
    /// </summary>
    public interface IActUsageHandlerSelector
    {
        /// <summary>
        /// Везвращает обработчик, способный обработать действие к указанной цели.
        /// </summary>
        /// <param name="attackTarget"> Цель применения действия. </param>
        /// <returns> Возвращает обработчик применения действия. </returns>
        IActUsageHandler GetHandler(IAttackTarget attackTarget);
    }
}