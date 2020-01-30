using JetBrains.Annotations;

namespace Zilon.Core.MapGenerators
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
        /// <seealso cref="SectorLevelSid"/>
        public RoomTransition([NotNull] string sectorSid, [NotNull] string sectorLevelSid)
        {
            if (string.IsNullOrEmpty(sectorSid))
            {
                throw new System.ArgumentException("message", nameof(sectorSid));
            }

            if (string.IsNullOrEmpty(sectorLevelSid))
            {
                throw new System.ArgumentException("message", nameof(sectorLevelSid));
            }

            SectorSid = sectorSid;
            SectorLevelSid = sectorLevelSid;
        }

        private RoomTransition()
        { 
            // Пустой конструктор для генерации выгода наружу.
            // Идентификаторы будут равны null.
        }

        /// <summary>
        /// Идентификатор схемы уровня сектора для перехода.
        /// </summary>
        /// <remarks>
        /// Если равно null, то означает, что переход выводит из подземелья (на глобалную карту).
        /// </remarks>
        public string SectorLevelSid { get; }

        /// <summary>
        /// Идентификатор схемы сектора для перехода.
        /// </summary>
        public string SectorSid { get; }

        /// <summary> Создаёт переход на поверхность. </summary>
        /// <returns> Возвращает экземпляр перехода, настроенного для перехода на поверхность. </returns>
        public static RoomTransition CreateGlobalExit()
        {
            return new RoomTransition();
        }

        /// <summary>
        /// Вывод строкого представления перехода.
        /// </summary>
        /// <returns>
        /// <see cref="string" />, который представляет переход.
        /// </returns>
        public override string ToString()
        {
            if (SectorLevelSid == null)
            {
                return "[Global]";
            }

            return SectorLevelSid;
        }
    }
}
