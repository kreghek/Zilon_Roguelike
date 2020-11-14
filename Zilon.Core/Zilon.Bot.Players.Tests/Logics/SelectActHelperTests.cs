using System;

using FluentAssertions;

using Moq;

using NUnit.Framework;

using Zilon.Core.Common;
using Zilon.Core.Components;
using Zilon.Core.Persons;
using Zilon.Core.Props;
using Zilon.Core.Schemes;
using Zilon.Core.Tests.Common.Schemes;

namespace Zilon.Bot.Players.Logics.Tests
{
    [TestFixture]
    [Parallelizable(ParallelScope.All)]
    public class SelectActHelperTests
    {
        [Test]
        public void SelectBestAct_HasResourceRequireActAndHasNoResource_SelectsDefault()
        {
            // ARRANGE

            var acts = new ITacticalAct[]
            {
                new TacticalAct(
                    new TestTacticalActScheme
                    {
                        Sid = "default",
                        Stats = new TestTacticalActStatsSubScheme
                        {
                            Effect = TacticalActEffectType.Damage,
                            Efficient = new Roll(3, 1),
                            Offence = new TestTacticalActOffenceSubScheme
                            {
                                ApRank = 1, Impact = ImpactType.Kinetic, Type = OffenseType.Tactical
                            }
                        }
                    },
                    new Roll(3, 1),
                    new Roll(3, 3),
                    null
                ),
                new TacticalAct(
                    new TestTacticalActScheme
                    {
                        Sid = "resource-required",
                        Stats = new TestTacticalActStatsSubScheme
                        {
                            Effect = TacticalActEffectType.Damage,
                            Efficient = new Roll(3, 3),
                            Offence = new TestTacticalActOffenceSubScheme
                            {
                                ApRank = 1, Impact = ImpactType.Kinetic, Type = OffenseType.Tactical
                            }
                        },
                        Constrains = new TestTacticalActConstrainsSubScheme
                        {
                            PropResourceType = "resource", PropResourceCount = 1
                        }
                    },
                    new Roll(3, 1),
                    new Roll(3, 3),
                    null
                )
            };

            var inventoryMock = new Mock<IPropStore>();
            inventoryMock.Setup(x => x.CalcActualItems()).Returns(Array.Empty<IProp>());
            var inventory = inventoryMock.Object;

            // ACT
            var factAct = SelectActHelper.SelectBestAct(acts, inventory);

            // ASSERT
            factAct.Scheme.Sid.Should().Be("default");
        }

        [Test]
        public void SelectBestAct_HasResourceRequireActAndHasNoInventory_SelectsDefault()
        {
            // ARRANGE

            var acts = new ITacticalAct[]
            {
                new TacticalAct(
                    new TestTacticalActScheme
                    {
                        Sid = "default",
                        Stats = new TestTacticalActStatsSubScheme
                        {
                            Effect = TacticalActEffectType.Damage,
                            Efficient = new Roll(3, 1),
                            Offence = new TestTacticalActOffenceSubScheme
                            {
                                ApRank = 1, Impact = ImpactType.Kinetic, Type = OffenseType.Tactical
                            }
                        }
                    },
                    new Roll(3, 1),
                    new Roll(3, 3),
                    null
                ),
                new TacticalAct(
                    new TestTacticalActScheme
                    {
                        Sid = "resource-required",
                        Stats = new TestTacticalActStatsSubScheme
                        {
                            Effect = TacticalActEffectType.Damage,
                            Efficient = new Roll(3, 3),
                            Offence = new TestTacticalActOffenceSubScheme
                            {
                                ApRank = 1, Impact = ImpactType.Kinetic, Type = OffenseType.Tactical
                            }
                        },
                        Constrains = new TestTacticalActConstrainsSubScheme
                        {
                            PropResourceType = "resource", PropResourceCount = 1
                        }
                    },
                    new Roll(3, 1),
                    new Roll(3, 3),
                    null
                )
            };

            // ACT
            var factAct = SelectActHelper.SelectBestAct(acts, null);

            // ASSERT
            factAct.Scheme.Sid.Should().Be("default");
        }
    }
}