using System.Collections;
using Moq;
using NUnit.Framework;
using Zilon.Logic.Persons;
using Zilon.Logic.Players;
using Zilon.Logic.Tactics.Initialization;
using Zilon.Logic.Tactics.Map;

namespace Zilon.Logic.Tests.Services
{
    class CombatServiceTestCaseGenerator
    {
        public static IEnumerable CreateCombatCorrectTestCases
        {
            get
            {
                yield return new TestCaseData(GetTwoGroupsData());
            }
        }

        private static CombatInitData GetTwoGroupsData()
        {
            return new CombatInitData
            {
                Map = new CombatMap(),
                Players = new[] {
                    new PlayerCombatInitData{
                        Player = CreateFakePlayer(),
                        Squads =new[]{
                            new Squad{
                                Persons = new[]{ CreatePerson() }
                            }
                        }
                    },
                    new PlayerCombatInitData{
                        Player = CreateFakePlayer(),
                        Squads =new[]{
                            new Squad{
                                Persons = new[]{ CreatePerson() }
                            }
                        }
                    }
                }
            };
        }

        private static Person CreatePerson()
        {
            var person = new Person { };
            return person;
        }

        private static IPlayer CreateFakePlayer()
        {
            var playerMock = new Mock<IPlayer>();
            return playerMock.Object;
        }
    }
}
