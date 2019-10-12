using System.Linq;
using System.Threading.Tasks;

using LightInject;

using Zilon.Core.Tactics;
using Zilon.Core.Tactics.Spatial;

namespace Zilon.Core.MassSectorGenerator.SectorValidators
{
    class ChestValidator : ISectorValidator
    {
        public Task Validate(ISector sector, Scope scopeContainer)
        {
            return Task.Run(() =>
            {
                // Сундуки не должны генерироваться на узлы, которые являются препятствием.
                // Сундуки не должны генерироваться на узлы с выходом.
                var containerManager = scopeContainer.GetInstance<IPropContainerManager>();
                var allContainers = containerManager.Items;
                foreach (var container in allContainers)
                {
                    // Проверяем, что сундук не стоит на препятствии.
                    var hex = (HexNode)container.Node;
                    if (hex.IsObstacle)
                    {
                        throw new SectorValidationException();
                    }

                    // Проверяем, что сундук не на клетке с выходом.
                    var transitionNodes = sector.Map.Transitions.Keys;
                    var chestOnTransitionNode = transitionNodes.Contains(container.Node);
                    if (chestOnTransitionNode)
                    {
                        throw new SectorValidationException();
                    }
                }
            });
        }
    }
}
