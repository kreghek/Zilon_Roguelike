using Zilon.Core.World;

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
        public RoomTransition(ISectorNode sectorNode)
        {
            SectorNode = sectorNode;
        }

        /// <summary>
        /// Узел сектора в графе биома.
        /// </summary>
        public ISectorNode SectorNode { get; }

        /// <summary>
        /// Вывод строкого представления перехода.
        /// </summary>
        /// <returns>
        /// <see cref="string" />, который представляет переход.
        /// </returns>
        public override string ToString()
        {
            return SectorNode.ToString();
        }
    }
}