using System.Linq;

using LightInject;

using Zilon.Core.Client;
using Zilon.Core.Commands;
using Zilon.Core.Tactics.Spatial;
using Zilon.Core.Tests.Common;

namespace Zilon.Core.Spec.Contexts
{
    public class CommonGameActionsContext : FeatureContextBase
    {
        public void MoveOnceActiveActor(OffsetCoords targetCoords)
        {
            var playerState = _container.GetInstance<IPlayerState>();
            var moveCommand = _container.GetInstance<ICommand>("move");
            var map = _container.GetInstance<IMap>();

            var targetNode = map.Nodes.Cast<HexNode>().SelectBy(targetCoords.X, targetCoords.Y);
            var nodeViewModel = new TestNodeViewModel
            {
                Node = targetNode
            };

            playerState.HoverViewModel = nodeViewModel;

            moveCommand.Execute();
        }

        public void UsePropByActiveActor(string propSid)
        {
            var useSelfCommand = _container.GetInstance<ICommand>("use-self");
            var inventoryState = _container.GetInstance<IInventoryState>();
            var actor = GetActiveActor();

            var selectedProp = actor.Person.Inventory.CalcActualItems().First(x => x.Scheme.Sid == propSid);

            var viewModel = new TestPropItemViewModel()
            {
                Prop = selectedProp
            };
            inventoryState.SelectedProp = viewModel;

            useSelfCommand.Execute();
        }
    }
}
