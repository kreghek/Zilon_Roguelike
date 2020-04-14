using System;
using System.Linq;
using System.Threading.Tasks;

using Zilon.Core.Tactics;
using Zilon.Core.Tactics.Spatial;

namespace Zilon.Core.MassSectorGenerator.SectorValidators
{
    /// <summary>
    /// Валидатор корректности расстановки моснтров в секторе.
    /// </summary>
    [System.Diagnostics.CodeAnalysis.SuppressMessage("Performance",
        "CA1812:Avoid uninstantiated internal classes",
        Justification = "Регистрируется в контейнере зависимостей через рефлексию.")]
    class MonsterValidator : ISectorValidator
    {
        public Task Validate(ISector sector, IServiceProvider scopeContainer)
        {
            return Task.Run(() =>
            {
                var staticObjectManager = sector.StaticObjectManager;
                var allContainers = staticObjectManager.Items;

                // Монстры не должны генерироваться на узлах с препятствием.
                // Монстры не должны генерироваться на узлах с сундуками.
                var actorManager = sector.ActorManager;
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
