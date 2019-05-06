using System;
using System.Collections.Generic;
using System.Linq;

using Zilon.Core.Client;
using Zilon.Core.Commands;
using Zilon.Core.Tactics;
using Zilon.Core.Tactics.Spatial;

namespace Zilon.Bot
{
    public class Interpreter
    {
        private readonly ISectorManager _sectorManager;
        private readonly ISectorUiState _sectorUiState;
        private readonly ICommand _moveCommand;

        public Interpreter(ISectorManager sectorManager, ISectorUiState sectorUiState,
            ICommand moveCommand)
        {
            _sectorManager = sectorManager;
            _sectorUiState = sectorUiState;
            _moveCommand = moveCommand;
        }

        public ICommand Process(string request)
        {
            var requestComponents = request.Split(' ');
            var method = requestComponents[0];
            var methdParams = requestComponents.Skip(1).ToArray();

            switch (method)
            {
                case "map":
                    PrintMap();
                    break;

                case "move":
                    return MoveToNode(methdParams[0]);
            }

            return null;
        }

        private void PrintMap()
        {
            var map = _sectorManager.CurrentSector.Map;
            var selectedNodes = new List<IMapNode>();
            var currentNode = _sectorUiState.ActiveActor.Actor.Node;

            foreach (var node in map.Nodes)
            {
                if (map.DistanceBetween(currentNode, node) <= 6)
                {
                    selectedNodes.Add(node);

                    Console.WriteLine(node);
                }
            }
        }

        private ICommand MoveToNode(string serializedNode)
        {
            var nodeCoords = serializedNode
                .Trim('(', ')')
                .Split(new[] { ',' }, StringSplitOptions.RemoveEmptyEntries);

            var map = _sectorManager.CurrentSector.Map;
            var node = map.Nodes.Single(x => ((HexNode)x).OffsetX == int.Parse(nodeCoords[0])
            && ((HexNode)x).OffsetY == int.Parse(nodeCoords[1]));

            var nodeViewModel = new NodeViewModel { Node = (HexNode)node };
            _sectorUiState.SelectedViewModel = nodeViewModel;

            return _moveCommand;
        }
    }
}
