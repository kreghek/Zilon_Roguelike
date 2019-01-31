using Zilon.Core.Tactics.Spatial;

namespace Zilon.Core.World
{
    /// <summary>
    /// Узел провинции в графе глобальной карте.
    /// </summary>
    public class GlobeRegionNode : HexNode
    {
        /// <summary>
        /// Конструктор узла провинции без сектора.
        /// </summary>
        /// <param name="x"> Координата X в сетке гексов. </param>
        /// <param name="y"> Координата Y в сетке гексов. </param>
        public GlobeRegionNode(int x, int y) : base(x, y)
        {
        }

        /// <summary>
        /// Конструктор узла провинции с сектором.
        /// </summary>
        /// <param name="x"> Координата X в сетке гексов. </param>
        /// <param name="y"> Координата Y в сетке гексов. </param>
        /// <param name="sectorSid"> Символьный идентификатор схемы сектора.
        /// Если равен null, то в узле нет сектора. </param>
        public GlobeRegionNode(int x, int y, string sectorSid) : base(x, y)
        {
            SectorSid = sectorSid;
        }

        /// <summary>
        /// Символьный идентификаторв сектора.
        /// </summary>
        public string SectorSid { get; }
    }
}
