﻿using System.Linq;

using LightInject;

using Moq;

using Zilon.Core.Client;
using Zilon.Core.Commands;
using Zilon.Core.Tactics;
using Zilon.Core.Tactics.Spatial;
using Zilon.Core.Tests.Common;

namespace Zilon.Core.Spec.Contexts
{
    public class CommonGameActionsContext : FeatureContextBase
    {
        public void MoveOnceActiveActor(OffsetCoords targetCoords)
        {
            var playerState = Container.GetInstance<IPlayerState>();
            var moveCommand = Container.GetInstance<ICommand>("move");
            var sectorManager = Container.GetInstance<ISectorManager>();

            var targetNode = sectorManager
                .CurrentSector
                .Map
                .Nodes
                .Cast<HexNode>()
                .SelectBy(targetCoords.X, targetCoords.Y);

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
            var useSelfCommand = Container.GetInstance<ICommand>("use-self");
            var inventoryState = Container.GetInstance<IInventoryState>();
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
            var playerState = Container.GetInstance<IPlayerState>();
            var sectorManager = Container.GetInstance<ISectorManager>();

            var map = sectorManager.CurrentSector.Map;
            var selectedNode = map.Nodes.Cast<HexNode>().Single(n => n.OffsetX == x && n.OffsetY == y);

            var nodeViewModelMock = new Mock<IMapNodeViewModel>();
            nodeViewModelMock.SetupGet(n => n.Node).Returns(selectedNode);
            var nodeViewModel = nodeViewModelMock.Object;

            playerState.HoverViewModel = nodeViewModel;
            playerState.SelectedViewModel = nodeViewModel;
        }
    }
}
