using Zilon.Core.Schemes;
using Zilon.Core.Tactics.Spatial;

namespace Zilon.Core.World
{
    /// <summary>
    /// Узел локации провинции в графе провинции.
    /// </summary>
    public sealed class GlobeRegionNode : HexNode
    {
        /// <summary>
        /// Конструктор узла провинции.
        /// </summary>
        /// <param name="x"> Координата X в сетке гексов. </param>
        /// <param name="y"> Координата Y в сетке гексов. </param>
        /// <param name="scheme"> Схема провинции. </param>
        public GlobeRegionNode(int x, int y, ILocationScheme scheme) : base(x, y)
        {
            Scheme = scheme;
        }

        /// <summary>
        /// Схема провинции.
        /// </summary>
        public ILocationScheme Scheme { get; }

        /// <summary>
        /// Является ли локация фрагментом города.
        /// </summary>
        public bool IsTown { get; set; }

        /// <summary>
        /// Является ли узел провинции пограничным с другой провинцией.
        /// </summary>
        /// <remarks>
        /// Нужен, чтобы покрыть требование, что подземелья и города не могут генерироваться на границе провинций.
        /// Нужен для оптимизации выбора узлов для построения переходов между провинциями.
        /// </remarks>
        public bool IsBorder { get; set; }

        /// <summary>
        /// Текущее состояние монстров в текущем узле. Может быть null.
        /// </summary>
        /// <remarks>
        /// Если не равно null, то указывает сектору, каких монстров нужно создать.
        /// </remarks>
        public GlobeRegionNodeMonsterState MonsterState { get; set; }

        /// <summary>
        /// Состояние разведки данного узла группой игрока.
        /// </summary>
        public GlobeNodeObservedState ObservedState { get; set; }
    }
}
