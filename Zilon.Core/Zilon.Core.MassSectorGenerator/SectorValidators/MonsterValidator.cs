using Zilon.Core.Tactics;

namespace Zilon.Core.MassSectorGenerator.SectorValidators
{
    /// <summary>
    ///     Валидатор корректности расстановки моснтров в секторе.
    /// </summary>
    [System.Diagnostics.CodeAnalysis.SuppressMessage("Performance",
        "CA1812:Avoid uninstantiated internal classes",
        Justification = "Регистрируется в контейнере зависимостей через рефлексию.")]
    internal class MonsterValidator : ISectorValidator
    {
        public Task Validate(ISector sector, IServiceProvider scopeContainer)
        {
            return Task.Run(() =>
            {
                IStaticObjectManager staticObjectManager = sector.StaticObjectManager;
                var allStaticObjects = staticObjectManager.Items;

                // Монстры не должны генерироваться на узлах с препятствием.
                // Монстры не должны генерироваться на узлах с сундуками.
                IActorManager actorManager = sector.ActorManager;
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