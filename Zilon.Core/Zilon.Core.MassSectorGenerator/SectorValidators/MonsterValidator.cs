using System.Linq;
using System.Threading.Tasks;

using LightInject;

using Zilon.Core.Tactics;
using Zilon.Core.Tactics.Spatial;

namespace Zilon.Core.MassSectorGenerator.SectorValidators
{
    /// <summary>
    /// Валидатор корректности расстановки моснтров в секторе.
    /// </summary>
    class MonsterValidator : ISectorValidator
    {
        public Task Validate(ISector sector, Scope scopeContainer)
        {
            return Task.Run(() =>
            {
                var containerManager = scopeContainer.GetInstance<IPropContainerManager>();
                var allContainers = containerManager.Items;

                // Монстры не должны генерироваться на узлах с препятствием.
                // Монстры не должны генерироваться на узлах с сундуками.
                var actorManager = scopeContainer.GetInstance<IActorManager>();
                var allMonsters = actorManager.Items;
                var containerNodes = allContainers.Select(x => x.Node);
                foreach (var actor in allMonsters)
                {
                    var hex = (HexNode)actor.Node;
                    if (hex.IsObstacle)
                    {
                        throw new SectorValidationException();
                    }

                    var monsterIsOnContainer = containerNodes.Contains(actor.Node);
                    if (monsterIsOnContainer)
                    {
                        throw new SectorValidationException();
                    }
                }
            });
        }
    }
}
