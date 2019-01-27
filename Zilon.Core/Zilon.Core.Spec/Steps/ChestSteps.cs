using System.Linq;

using FluentAssertions;

using TechTalk.SpecFlow;

using Zilon.Core.Spec.Contexts;
using Zilon.Core.Tactics;

using LightInject;
using Zilon.Core.Tactics.Spatial;
using Zilon.Core.Tests.Common;
using Zilon.Core.Tests.Common.Schemes;
using Zilon.Core.Schemes;
using System.Collections.Generic;
using Moq;
using Zilon.Core.Props;
using Zilon.Core.Client;

namespace Zilon.Core.Spec.Steps
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
            var schemeService = Context.Container.GetInstance<ISchemeService>();
            var containerManager = Context.Container.GetInstance<IPropContainerManager>();
            var sectorManager = Context.Container.GetInstance<ISectorManager>();

            var nodeCoords = new OffsetCoords(chestPosX, chestPosY);
            var node = sectorManager.CurrentSector.Map.Nodes.Cast<HexNode>().SelectBy(nodeCoords.X, nodeCoords.Y);

            var dropProps = new List<IProp>();
            foreach (var tableRow in table.Rows)
            {
                tableRow.TryGetValue("prop", out var propSchemeSid);
                tableRow.TryGetValue("count", out var resourceCount);

                var propScheme = schemeService.GetScheme<IPropScheme>(propSchemeSid);

                dropProps.Add(new Resource(propScheme, int.Parse(resourceCount)));
            }

            var dropResolverMock = new Mock<IDropResolver>();
            dropResolverMock.Setup(x => x.GetProps(It.IsAny<IEnumerable<IDropTableScheme>>()))
                .Returns(dropProps.ToArray());
            var dropResolver = dropResolverMock.Object;

            var chest = new DropTablePropChest(node, new DropTableScheme[0], dropResolver, chestId);

            containerManager.Add(chest);
        }


        [Then(@"В выбранном сундуке лут")]
        public void ThenВВыбранномСундукеЛут(Table table)
        {
            var playerState = Context.Container.GetInstance<IPlayerState>();
            var selectedChest = (playerState.HoverViewModel as IContainerViewModel).Container;

            // lootProps будет изменяться
            var lootProps = selectedChest.Content.CalcActualItems().ToList();

            foreach (var tableRow in table.Rows)
            {
                tableRow.TryGetValue("prop", out var expectedPropSid);
                tableRow.TryGetValue("count", out var expectedResourceCount);

                var factLootProps = lootProps.Where(x => x.Scheme.Sid == expectedPropSid);
                var factLootResources = factLootProps.Cast<Resource>();
                var factLootResource = factLootResources.FirstOrDefault(x => x.Count == int.Parse(expectedResourceCount));

                factLootResource.Should().NotBeNull();
                factLootResource.Scheme.Sid.Should().Be(expectedPropSid);
                factLootResource.Count.Should().Be(int.Parse(expectedResourceCount));
                lootProps.Remove(factLootResource);
            }
        }

    }
}