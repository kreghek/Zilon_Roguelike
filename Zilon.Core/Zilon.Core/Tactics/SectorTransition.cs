using System;
using JetBrains.Annotations;
using Zilon.Core.Graphs;

namespace Zilon.Core.Tactics
{
    ///// <summary>
    ///// Данные по переходу между секторами.
    ///// </summary>
    ///// <remarks>
    ///// Используется для обозначения переходов между секторами.
    ///// </remarks>
    //public sealed class SectorTransition
    //{
    //    /// <summary>
    //    /// Создаение экземпляра перехода.
    //    /// </summary>
    //    /// <param name="sectorSid">
    //    /// Идентификатор сектора в рамках указанной локации.
    //    /// Если указано null, означает, что будет переход "на поверхность".
    //    /// </param>
    //    /// <seealso cref="SectorLevelSid"/>
    //    public SectorTransition([NotNull] string sectorSid, [NotNull] string sectorLevelSid)
    //    {
    //        if (string.IsNullOrEmpty(sectorSid))
    //        {
    //            throw new System.ArgumentException("message", nameof(sectorSid));
    //        }

    //        if (string.IsNullOrEmpty(sectorLevelSid))
    //        {
    //            throw new System.ArgumentException("message", nameof(sectorLevelSid));
    //        }

    //        SectorSid = sectorSid;
    //        SectorLevelSid = sectorLevelSid;
    //    }

    //    private SectorTransition()
    //    {
    //        // Пустой конструктор для генерации выгода наружу.
    //        // Идентификаторы будут равны null.
    //    }

    //    /// <summary>
    //    /// Идентификатор схемы уровня сектора для перехода.
    //    /// </summary>
    //    /// <remarks>
    //    /// Если равно null, то означает, что переход выводит из подземелья (на глобалную карту).
    //    /// </remarks>
    //    public string SectorLevelSid { get; }

    //    /// <summary>
    //    /// Идентификатор схемы сектора для перехода.
    //    /// </summary>
    //    public string SectorSid { get; }

    //    /// <summary> Создаёт переход на поверхность. </summary>
    //    /// <returns> Возвращает экземпляр перехода, настроенного для перехода на поверхность. </returns>
    //    public static SectorTransition CreateGlobalExit()
    //    {
    //        return new SectorTransition();
    //    }

    //    /// <summary>
    //    /// Вывод строкого представления перехода.
    //    /// </summary>
    //    /// <returns>
    //    /// <see cref="string" />, который представляет переход.
    //    /// </returns>
    //    public override string ToString()
    //    {
    //        if (SectorLevelSid == null)
    //        {
    //            return "[Global]";
    //        }

    //        return SectorLevelSid;
    //    }
    //}

    /// <summary>
    /// Переход между секторами.
    /// </summary>
    public interface ISectorTransition
    {
        /// <summary>
        /// Выполнить переход.
        /// </summary>
        void Make();
    }

    /// <summary>
    /// Отложеный переход. Содержит метаданные для генерации следующего сектора.
    /// </summary>
    public sealed class DeferredSectorTransition : ISectorTransition
    {
        public DeferredSectorTransition(string sectorLevelSid, string sectorSid)
        {
            SectorLevelSid = sectorLevelSid ?? throw new ArgumentNullException(nameof(sectorLevelSid));
            SectorSid = sectorSid ?? throw new ArgumentNullException(nameof(sectorSid));
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

        /// <inheritdoc/>
        public void Make()
        {
            throw new System.NotImplementedException();
        }
    }

    /// <summary>
    /// Переход по ссылке в существующий сектора.
    /// </summary>
    public sealed class RefSectorTransition : ISectorTransition
    {
        public RefSectorTransition(ISector sector, IGraphNode node)
        {
            Sector = sector ?? throw new ArgumentNullException(nameof(sector));
            Node = node ?? throw new ArgumentNullException(nameof(node));
        }

        public ISector Sector { get; }

        public IGraphNode Node { get; }

        /// <inheritdoc/>
        public void Make()
        {
            throw new System.NotImplementedException();
        }
    }
}
