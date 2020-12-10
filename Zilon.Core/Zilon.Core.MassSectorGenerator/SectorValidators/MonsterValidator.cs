using System;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using System.Threading.Tasks;

using Zilon.Core.Tactics;
using Zilon.Core.Tactics.Spatial;

namespace Zilon.Core.MassSectorGenerator.SectorValidators
{
    /// <summary>
    /// Валидатор корректности расстановки моснтров в секторе.
    /// </summary>
    [SuppressMessage("Performance",
        "CA1812:Avoid uninstantiated internal classes",
        Justification = "Регистрируется в контейнере зависимостей через рефлексию.")]
    internal class MonsterValidator : ISectorValidator
    {
        public Task Validate(ISector sector, IServiceProvider scopeContainer)
        {
            return Task.Run(() =>
            {
                var staticObjectManager = sector.StaticObjectManager;
                var allStaticObjects = staticObjectManager.Items;

                // Монстры не должны генерироваться на узлах с препятствием.
                // Монстры не должны генерироваться на узлах с сундуками.
                var actorManager = sector.ActorManager;
                var allMonsters = actorManager.Items;
                var staticObjectNodes = allStaticObjects.Select(x => x.Node);
                foreach (var actor in allMonsters)
                {
                    var monsterIsOnStaticObject = staticObjectNodes.Contains(actor.Node);
                    if (monsterIsOnStaticObject)
                    {
                        throw new SectorValidationException();
                    }
                }
            });
        }
    }
}