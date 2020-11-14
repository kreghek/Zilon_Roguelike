using System.Collections.Generic;
using Zilon.Core.Tactics.Spatial;

namespace Zilon.Core.MapGenerators.RoomStyle
{
    /// <summary>
    /// Объект комнаты для генерации сектора.
    /// </summary>
    public class Room
    {
        public Room()
        {
            Nodes = new List<HexNode>();
            Transitions = new List<RoomTransition>();
        }

        /// <summary>
        /// Координата X в матрице комнат.
        /// </summary>
        public int PositionX { get; set; }

        /// <summary>
        /// Координата Y в матрице комнат.
        /// </summary>
        public int PositionY { get; set; }

        /// <summary>
        /// Ширина комнаты.
        /// </summary>
        public int Width { get; set; }

        /// <summary>
        /// Высота комнаты комнаты.
        /// </summary>
        public int Height { get; set; }

        /// <summary>
        /// Узлы данной комнаты.
        /// </summary>
        public List<HexNode> Nodes { get; }

        /// <summary>
        /// Идентификаторы секторов в текущей локации,
        /// в которые возможен переход из данной комнаты.
        /// </summary>
        /// <remarks>
        /// Этот набор является подмножеством идентфикаторов секторов
        /// из схемы строящегося сектора
        /// </remarks>
        public List<RoomTransition> Transitions { get; }

        /// <summary>
        /// Признак того, что комната является стартовой в секторе.
        /// </summary>
        public bool IsStart { get; set; }

        public override string ToString()
        {
            return $"({PositionX}, {PositionY})";
        }
    }
}