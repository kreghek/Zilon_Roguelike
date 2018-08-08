using NUnit.Framework;
using Zilon.Core.Persons;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Zilon.Core.Tactics;
using Moq;
using FluentAssertions;
using Zilon.Core.Schemes;

namespace Zilon.Core.Persons.Tests
{
    [TestFixture()]
    public class PerkResolverTests
    {
        /// <summary>
        /// Тест проверяет, что при выполнении работ перка вызывается событие повышения уровня перка.
        /// </summary>
        [Test()]
        public void ApplyProgressTest()
        {
            // ARRANGE
            var perkScheme = new PerkScheme
            {
                Levels = new[] {
                    new PerkLevelSubScheme{
                        Jobs = new[]{
                            new JobSubScheme{
                                Type = JobType.Defeats,
                                Value = 1
                            }
                        }
                    }
                }
            };

            var perkJobMock = new Mock<IJob>();
            perkJobMock.SetupProperty(x => x.Progress, 1);
            perkJobMock.SetupGet(x => x.Scheme)
                .Returns(perkScheme.Levels[0].Jobs[0]);
            var isComplete = false;
            perkJobMock.SetupGet(x => x.IsComplete).Returns(() => isComplete);
            perkJobMock.SetupSet(x => x.IsComplete).Callback(x => isComplete = x);
            var perkJob = perkJobMock.Object;


            var progressMock = new Mock<IJobProgress>();
            progressMock.Setup(x => x.ApplyToJobs(It.IsAny<IEnumerable<IJob>>()))
                .Returns(new IJob[] { perkJob });
            var progress = progressMock.Object;

            var perkMock = new Mock<IPerk>();
            perkMock.SetupGet(x => x.Scheme)
                .Returns(perkScheme);
            perkMock.SetupGet(x => x.CurrentLevel)
                .Returns(PerkLevel.Zero);
            var perk = perkMock.Object;

            var perks = new IPerk[] {
                perk
            };
            var evolutionDataMock = new Mock<IEvolutionData>();
            evolutionDataMock.SetupGet(x => x.Perks)
                .Returns(perks);
            var evolutionData = evolutionDataMock.Object;

            var perkResolver = new PerkResolver();




            // ACT
            perkResolver.ApplyProgress(progress, evolutionData);



            // ASSERT
            evolutionDataMock.Verify(x => x.PerkLevelUp(It.IsAny<IPerk>()));
        }
    }
}