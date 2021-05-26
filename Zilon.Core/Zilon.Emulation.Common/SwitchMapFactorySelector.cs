using Zilon.Core.MapGenerators;
using Zilon.Core.MapGenerators.CellularAutomatonStyle;
using Zilon.Core.MapGenerators.OpenStyle;
using Zilon.Core.MapGenerators.RoomStyle;

namespace Zilon.Emulation.Common
{
    /// <summary>
    /// Реализация селектора фабрик на основе переключения по типу фабрики.
    /// </summary>
    public sealed class SwitchMapFactorySelector : SwitchMapFactorySelectorBase
    {
        public SwitchMapFactorySelector(RoomMapFactory roomMapFactory,
            CellularAutomatonMapFactory cellularAutomatonMapFactory,
            OpenMapFactory openMapFactory)
        {
            RoomMapFactory = roomMapFactory;
            CellularAutomatonMapFactory = cellularAutomatonMapFactory;
            OpenMapFactory = openMapFactory;
        }

        protected override IMapFactory CellularAutomatonMapFactory { get; }

        protected override IMapFactory OpenMapFactory { get; }

        protected override IMapFactory RoomMapFactory { get; }
    }
}