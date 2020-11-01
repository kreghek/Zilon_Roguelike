using System;
using System.Collections.Generic;
using System.Linq;
using Zilon.Core.Client;
using Zilon.Core.Players;
using Zilon.Core.Props;
using Zilon.Core.Schemes;
using Zilon.Core.Specs.Contexts;
using Zilon.Core.StaticObjectModules;
using Zilon.Core.Tactics;

namespace Zilon.Core.Specs.Steps
{
    [Binding]
    public sealed class ChestSteps : GenericStepsBase<CommonGameActionsContext>
    {
        public ChestSteps(CommonGameActionsContext context) : base(context)
        {
        }

        [Given(@"Есть сундук Id:(.*) в ячейке \((.*), (.*)\) со случайным лутом")]
        public void GivenЕстьСундукIdВЯчейкеСоСлучайнымЛутом(int chestId, int chestPosX, int chestPosY, Table table)
        {
            var schemeService = Context.ServiceProvider.GetRequiredService<ISchemeService>();
            var player = Context.ServiceProvider.GetRequiredService<IPlayer>();
            var sector = player.SectorNode.Sector;
            var staticObjectManager = sector.StaticObjectManager;

            OffsetCoords nodeCoords = new OffsetCoords(chestPosX, chestPosY);
            var node = sector.Map.Nodes.SelectByHexCoords(nodeCoords.X, nodeCoords.Y);

            List<IProp> dropProps = new List<IProp>();
            foreach (var tableRow in table.Rows)
            {
                tableRow.TryGetValue("prop", out var propSchemeSid);
                tableRow.TryGetValue("count", out var resourceCount);

                var propScheme = schemeService.GetScheme<IPropScheme>(propSchemeSid);

                dropProps.Add(new Resource(propScheme, int.Parse(resourceCount)));
            }

            var dropResolverMock = new Mock<IDropResolver>();
            dropResolverMock.Setup(x => x.Resolve(It.IsAny<IEnumerable<IDropTableScheme>>()))
                .Returns(dropProps.ToArray());
            var dropResolver = dropResolverMock.Object;

            DropTablePropChest chest = new DropTablePropChest(Array.Empty<DropTableScheme>(), dropResolver);
            StaticObject staticObject = new StaticObject(node, chest.Purpose, chestId);
            staticObject.AddModule<IPropContainer>(chest);

            staticObjectManager.Add(staticObject);
        }

        [Then(@"В выбранном сундуке лут")]
        public void ThenВВыбранномСундукеЛут(Table table)
        {
            var playerState = Context.ServiceProvider.GetRequiredService<ISectorUiState>();
            IStaticObject selectedChest = (playerState.HoverViewModel as IContainerViewModel).StaticObject;

            // lootProps будет изменяться
            List<IProp> lootProps = selectedChest.GetModule<IPropContainer>().Content.CalcActualItems().ToList();

            foreach (var tableRow in table.Rows)
            {
                tableRow.TryGetValue("prop", out var expectedPropSid);
                tableRow.TryGetValue("count", out var expectedResourceCount);

                IEnumerable<IProp> factLootProps = lootProps.Where(x => x.Scheme.Sid == expectedPropSid);
                IEnumerable<Resource> factLootResources = factLootProps.Cast<Resource>();
                Resource factLootResource =
                    factLootResources.FirstOrDefault(x => x.Count == int.Parse(expectedResourceCount));

                factLootResource.Should().NotBeNull();
                factLootResource.Scheme.Sid.Should().Be(expectedPropSid);
                factLootResource.Count.Should().Be(int.Parse(expectedResourceCount));
                lootProps.Remove(factLootResource);
            }
        }
    }
}