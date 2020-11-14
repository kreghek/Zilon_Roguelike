using System.Linq;
using Zilon.Core.Client;
using Zilon.Core.Commands;
using Zilon.Core.PersonModules;
using Zilon.Core.Players;
using Zilon.Core.Props;
using Zilon.Core.Tactics;
using Zilon.Core.Tactics.Spatial;
using Zilon.Core.Tests.Common;

namespace Zilon.Core.Specs.Contexts
{
    public class CommonGameActionsContext : FeatureContextBase
    {
        public void MoveOnceActiveActor(OffsetCoords targetCoords)
        {
            var playerState = ServiceProvider.GetRequiredService<ISectorUiState>();
            var moveCommand = ServiceProvider.GetRequiredService<MoveCommand>();
            var player = ServiceProvider.GetRequiredService<IPlayer>();

            var sector = player.SectorNode.Sector;

            var targetNode = sector
                .Map
                .Nodes
                .SelectByHexCoords(targetCoords.X, targetCoords.Y);

            TestNodeViewModel nodeViewModel = new TestNodeViewModel {Node = targetNode};

            playerState.HoverViewModel = nodeViewModel;
            playerState.SelectedViewModel = nodeViewModel;

            moveCommand.Execute();
        }

        public void UsePropByActiveActor(string propSid)
        {
            var useSelfCommand = ServiceProvider.GetRequiredService<UseSelfCommand>();
            var inventoryState = ServiceProvider.GetRequiredService<IInventoryState>();
            IActor actor = GetActiveActor();

            IProp selectedProp = actor.Person.GetModule<IInventoryModule>().CalcActualItems()
                .First(x => x.Scheme.Sid == propSid);

            TestPropItemViewModel viewModel = new TestPropItemViewModel {Prop = selectedProp};
            inventoryState.SelectedProp = viewModel;

            useSelfCommand.Execute();
        }

        internal void ClickOnNode(int x, int y)
        {
            var playerState = ServiceProvider.GetRequiredService<ISectorUiState>();
            var player = ServiceProvider.GetRequiredService<IPlayer>();

            var sector = player.SectorNode.Sector;

            var map = sector.Map;
            var selectedNode = map.Nodes.Cast<HexNode>().Single(n => n.OffsetCoords.X == x && n.OffsetCoords.Y == y);

            var nodeViewModelMock = new Mock<IMapNodeViewModel>();
            nodeViewModelMock.SetupGet(n => n.Node).Returns(selectedNode);
            var nodeViewModel = nodeViewModelMock.Object;

            playerState.HoverViewModel = nodeViewModel;
            playerState.SelectedViewModel = nodeViewModel;
        }
    }
}