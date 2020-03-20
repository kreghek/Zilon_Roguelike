using System;

using Zilon.Core.Schemes;
using Zilon.Core.Tactics;
using Zilon.Core.Tactics.Spatial;

namespace Zilon.Core.World
{
    /// <summary>
    /// Узел локации провинции в графе провинции.
    /// </summary>
    public sealed class ProvinceNode : HexNode
    {
        private GlobeNodeObservedState _observedState;

        /// <summary>
        /// Конструктор узла провинции.
        /// </summary>
        /// <param name="x"> Координата X в сетке гексов. </param>
        /// <param name="y"> Координата Y в сетке гексов. </param>
        /// <param name="scheme"> Схема провинции. </param>
        public ProvinceNode(int x, int y, ILocationScheme scheme) : base(x, y)
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
        /// Является ли город победным узлом.
        /// </summary>
        public bool IsHome { get; set; }

        /// <summary>
        /// Является ли узел провинции пограничным с другой провинцией.
        /// </summary>
        /// <remarks>
        /// Нужен, чтобы покрыть требование, что подземелья и города не могут генерироваться на границе провинций.
        /// Нужен для оптимизации выбора узлов для построения переходов между провинциями.
        /// </remarks>
        public bool IsBorder { get; set; }

        /// <summary>
        /// Является ли узел стартовым для группы игрока.
        /// </summary>
        public bool IsStart { get; set; }

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
        public GlobeNodeObservedState ObservedState
        {
            get => _observedState;
            set
            {
                var oldValue = _observedState;
                _observedState = value;
                if (_observedState != oldValue)
                {
                    DoObservedStateChanged();
                }
            }
        }

        /// <summary>
        /// Выстреливает, когда изменяется состояние исследования узла.
        /// </summary>
        public event EventHandler ObservedStateChanged;

        /// <summary>
        /// Сгенерированный сектор для этого узла провинции.
        /// </summary>
        public ISector Sector { get; private set; }

        /// <summary>
        /// Связывает узлен провинции с сектором, который нужно прогружать для этого узла.
        /// </summary>
        /// <param name="sector"></param>
        public void BindSector(ISector sector)
        {
            Sector = sector;
        }

        private void DoObservedStateChanged()
        {
            ObservedStateChanged?.Invoke(this, new EventArgs());
        }
    }
}
