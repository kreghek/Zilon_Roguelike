using LightInject;

using Zilon.Core.MapGenerators;

namespace Zilon.Emulation.Common
{
    /// <summary>
    /// Реализация селектора фабрик на основе переключения по типу фабрики.
    /// </summary>
    public sealed class LightInjectSwitchMapFactorySelector : SwitchMapFactorySelectorBase
    {
        public LightInjectSwitchMapFactorySelector([Inject("room")] IMapFactory roomMapFactory,
            [Inject("cellular-automaton")] IMapFactory cellularAutomatonMapFactory)
        {
            RoomMapFactory = roomMapFactory;
            CellularAutomatonMapFactory = cellularAutomatonMapFactory;
        }

        protected override IMapFactory CellularAutomatonMapFactory { get; }
        protected override IMapFactory RoomMapFactory { get; }
    }
}
