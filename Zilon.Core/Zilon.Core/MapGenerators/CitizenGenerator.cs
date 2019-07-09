using System.Collections.Generic;
using System.Linq;

using Zilon.Core.PersonDialogs;
using Zilon.Core.Persons;
using Zilon.Core.Players;
using Zilon.Core.Schemes;
using Zilon.Core.Tactics;
using Zilon.Core.Tactics.Spatial;

namespace Zilon.Core.MapGenerators
{
    public sealed class CitizenGenerator : ICitizenGenerator
    {
        private readonly ISchemeService _schemeService;
        private readonly IDropResolver _dropResolver;
        private readonly IActorManager _actorManager;
        private readonly ICitizenGeneratorRandomSource _citizenGeneratorRandomSource;

        /// <summary>
        /// Конструктор генератора жителей.
        /// </summary>
        /// <param name="schemeService"> Сервис схем. </param>
        /// <param name="dropResolver"> Служба работы с таблицами дропа. Нужна для создания торговцев. </param>
        /// <param name="actorManager"> Менеджер актёров. </param>
        public CitizenGenerator(ISchemeService schemeService,
            IDropResolver dropResolver,
            IActorManager actorManager,
            ICitizenGeneratorRandomSource citizenGeneratorRandomSource)
        {
            _schemeService = schemeService ?? throw new System.ArgumentNullException(nameof(schemeService));
            _dropResolver = dropResolver ?? throw new System.ArgumentNullException(nameof(dropResolver));
            _actorManager = actorManager ?? throw new System.ArgumentNullException(nameof(actorManager));
            _citizenGeneratorRandomSource = citizenGeneratorRandomSource ?? throw new System.ArgumentNullException(nameof(citizenGeneratorRandomSource));
        }

        public void CreateCitizens(ISector sector, IBotPlayer botPlayer, IEnumerable<MapRegion> citizenRegions)
        {
            var map = sector.Map;
            foreach (var region in citizenRegions)
            {
                // 1 к 9, потому что это наиболее удобное соотношение
                // для размещения неподвижных блокирующих объектов
                // без преграждения выходов.
                var citizenCount = region.Nodes.Count() / 9f;

                var availableNodes = from node in region.Nodes
                                     where !map.Transitions.Keys.Contains(node)
                                     select node;

                var openNodes = new List<IMapNode>(availableNodes);
                for (var i = 0; i < citizenCount; i++)
                {
                    //TODO Объединить этот блок с генератором сундуков, как дубликат
                    // Выбрать из коллекции доступных узлов
                    var rollIndex = _citizenGeneratorRandomSource.RollNodeIndex(openNodes.Count);
                    var objectNode = MapRegionHelper.FindNonBlockedNode(openNodes[rollIndex], map, openNodes);
                    if (objectNode == null)
                    {
                        // В этом случае будет сгенерировано на одного жителя меньше.
                        // Узел, с которого не удаётся найти подходящий узел, удаляем,
                        // Чтобы больше его не анализировать, т.к. всё равно будет такой же неудачный исход.
                        openNodes.Remove(openNodes[rollIndex]);
                        continue;
                    }

                    openNodes.Remove(objectNode);
                    var traderDropTable = _schemeService.GetScheme<IDropTableScheme>("trader");

                    var rollCitizenType = _citizenGeneratorRandomSource.RollCitizenType();

                    switch (rollCitizenType)
                    {
                        case CitizenType.Unintresting:
                            CreateCitizen(objectNode, botPlayer);
                            break;

                        case CitizenType.Trader:
                            CreateCitizen(traderDropTable, objectNode, botPlayer);
                            break;

                        case CitizenType.QuestGiver:
                            CreateCitizen(DialogFactory.Create(), objectNode, botPlayer);
                            break;

                        default:
                            //TODO Завести тип исключения на генерацию персонажей мирных жителей.
                            throw new System.Exception();
                    }
                }
            }
        }

        private IActor CreateCitizen(IDropTableScheme traderDropTable, IMapNode startNode, IBotPlayer botPlayer)
        {
            var person = new CitizenPerson(traderDropTable, _dropResolver);
            var actor = new Actor(person, botPlayer, startNode);
            _actorManager.Add(actor);
            return actor;
        }

        private IActor CreateCitizen(IMapNode startNode, IBotPlayer botPlayer)
        {
            var person = new CitizenPerson();
            var actor = new Actor(person, botPlayer, startNode);
            _actorManager.Add(actor);
            return actor;
        }

        private IActor CreateCitizen(Dialog dialog, IMapNode startNode, IBotPlayer botPlayer)
        {
            var person = new CitizenPerson(dialog);
            var actor = new Actor(person, botPlayer, startNode);
            _actorManager.Add(actor);
            return actor;
        }
    }
}
