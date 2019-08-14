using System;

using JetBrains.Annotations;

namespace Zilon.Core.Tactics.Spatial
{
    /// <summary>
    /// Регион карты - это тематическое объединение узлов карты.
    /// </summary>
    /// <remarks>
    /// Обычно используется для обозначения комнат или других замкнутых пространств
    /// внутри карты для внутренних нужд:
    /// 1. Для распределения лута.
    /// 2. Для распределения монстров.
    /// 3. Для ориентации монстров внутри карты. Например, для патрулирования комнат.
    /// </remarks>
    public class MapRegion
    {
        public MapRegion(int id, [NotNull][ItemNotNull] IMapNode[] nodes)
        {
            Id = id;
            Nodes = nodes ?? throw new ArgumentNullException(nameof(nodes));
        }

        public IMapNode[] Nodes { get; }

        public int Id { get; }

        public bool IsOut { get; set; }

        public bool IsStart { get; set; }

        public IMapNode[] ExitNodes { get; set; }

        public override string ToString()
        {
            return $"{Id}, Nodes: {Nodes.Length}";
        }
    }
}
