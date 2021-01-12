﻿using NUnit.Framework;
using Zilon.Bot.Players.Logics;
using System;
using System.Collections.Generic;
using System.Text;
using Moq;
using Zilon.Core.Tactics.Behaviour.Bots;
using Zilon.Core.Tactics;
using Zilon.Core.Tactics.Behaviour;
using FluentAssertions;
using Zilon.Core.Tactics.Spatial;
using Zilon.Core.Persons;
using Zilon.Core.PersonModules;
using Zilon.Core.Graphs;
using System.Linq;

namespace Zilon.Bot.Players.Logics.Tests
{
    [TestFixture()]
    public class ExploreLogicStateTests
    {
        [Test()]
        public void GetTask_FirstCall_ReturnsMoveTask()
        {
            // ARRANGE

            var mapNodes = new[] { Mock.Of<IGraphNode>(), Mock.Of<IGraphNode>() };

            var decisionSource = Mock.Of<IDecisionSource>(x=>x.SelectTargetRoamingNode(It.IsAny<IEnumerable<IGraphNode>>()) == mapNodes.First());

            var logic = new ExploreLogicState(decisionSource);

            var actor = Mock.Of<IActor>(actor => actor.Person == Mock.Of<IPerson>(
                person => person.HasModule(nameof(IFowData)) == true &&
                    person.GetModule<IFowData>(nameof(IFowData)) == Mock.Of<IFowData>(
                        fowModule => fowModule.GetSectorFowData(It.IsAny<ISector>()) == Mock.Of<ISectorFowData>(
                            sectorFowData => sectorFowData.GetFowNodeByState(SectorMapNodeFowState.Explored) == Array.Empty<SectorMapFowNode>().AsEnumerable()
                            )
                )) &&
                actor.Node == mapNodes.First()
           );

            var context = Mock.Of<ISectorTaskSourceContext>(x=>x.Sector == Mock.Of<ISector>(
                sector => sector.Map == Mock.Of<ISectorMap>(
                    map => map.Nodes == mapNodes &&
                        map.Transitions == new Dictionary<IGraphNode, Core.MapGenerators.SectorTransition>() &&
                        map.GetNext(It.IsAny<IGraphNode>()) == mapNodes.Skip(1) &&
                        map.IsPositionAvailableFor(It.IsAny<IGraphNode>(), It.IsAny<IActor>()) == true
                    )
                ));

            var strategyData = Mock.Of<ILogicStrategyData>();

            // ACT

            var factTask = logic.GetTask(actor, context, strategyData);

            // ASSERT
            factTask.Should().BeOfType<MoveTask>();
        }
    }
}