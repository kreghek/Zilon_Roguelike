using JetBrains.Annotations;

namespace Zilon.Core.MapGenerators.RoomStyle
{
    /// <summary>
    /// Данные по переходу из данной комнаты.
    /// </summary>
    /// <remarks>
    /// Используется для обозначения переходов между секторами
    /// или выхода из подземелья.
    /// </remarks>
    public sealed class RoomTransition
    {
        /// <summary>
        /// Создаение экземпляра перехода.
        /// </summary>
        /// <param name="sectorSid">
        /// Идентификатор сектора в рамках указанной локации.
        /// Если указано null, означает, что будет переход "на поверхность".
        /// </param>
        /// <seealso cref="SectorSid"/>
        public RoomTransition([CanBeNull] string sectorSid)
        {
            SectorSid = sectorSid;
        }

        /// <summary>
        /// Идентификатор сектора для перехода.
        /// </summary>
        /// <remarks>
        /// Если равно null, то означает, что переход выводит из подземелья (на глобалную карту).
        /// </remarks>
        public string SectorSid { get; }

        /// <summary> Создаёт переход на поверхность. </summary>
        /// <returns> Возвращает экземпляр перехода, настроенного для перехода на поверхность. </returns>
        public static RoomTransition CreateGlobalExit()
        {
            return new RoomTransition(null);
        }
    }
}
