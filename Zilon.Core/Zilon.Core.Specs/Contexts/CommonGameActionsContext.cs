using System.Linq;

using Microsoft.Extensions.DependencyInjection;

using Moq;

using Zilon.Core.Client;
using Zilon.Core.Commands;
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
            var sectorManager = ServiceProvider.GetRequiredService<ISectorManager>();

            var targetNode = sectorManager
                .CurrentSector
                .Map
                .Nodes
                .SelectByHexCoords(targetCoords.X, targetCoords.Y);

            var nodeViewModel = new TestNodeViewModel
            {
                Node = targetNode
            };

            playerState.HoverViewModel = nodeViewModel;
            playerState.SelectedViewModel = nodeViewModel;

            moveCommand.Execute();
        }

        public void UsePropByActiveActor(string propSid)
        {
            var useSelfCommand = ServiceProvider.GetRequiredService<UseSelfCommand>();
            var inventoryState = ServiceProvider.GetRequiredService<IInventoryState>();
            var actor = GetActiveActor();

            var selectedProp = actor.Person.Inventory.CalcActualItems().First(x => x.Scheme.Sid == propSid);

            var viewModel = new TestPropItemViewModel()
            {
                Prop = selectedProp
            };
            inventoryState.SelectedProp = viewModel;

            useSelfCommand.Execute();
        }

        internal void ClickOnNode(int x, int y)
        {
            var playerState = ServiceProvider.GetRequiredService<ISectorUiState>();
            var sectorManager = ServiceProvider.GetRequiredService<ISectorManager>();

            var map = sectorManager.CurrentSector.Map;
            var selectedNode = map.Nodes.Cast<HexNode>().Single(n => n.OffsetCoords.X == x && n.OffsetCoords.Y == y);

            var nodeViewModelMock = new Mock<IMapNodeViewModel>();
            nodeViewModelMock.SetupGet(n => n.Node).Returns(selectedNode);
            var nodeViewModel = nodeViewModelMock.Object;

            playerState.HoverViewModel = nodeViewModel;
            playerState.SelectedViewModel = nodeViewModel;
        }
    }
}
