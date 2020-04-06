using System.Collections.Generic;

using Moq;

using NUnit.Framework;

using Zilon.Core.Persons;
using Zilon.Core.Schemes;
using Zilon.Core.Tests.Common.Schemes;

namespace Zilon.Core.Tests.Persons
{
    [TestFixture][Parallelizable(ParallelScope.All)]
    public class PerkResolverTests
    {
        /// <summary>
        /// Тест проверяет, что при выполнении работ перка вызывается метод повышения уровня перка.
        /// </summary>
        [Test]
        public void ApplyProgress_AllJobsAreDone_EvolutionDataLevelUpCalled()
        {
            // ARRANGE
            var perkScheme = new PerkScheme
            {
                Levels = new[] {
                    new PerkLevelSubScheme{
                        Jobs = new IJobSubScheme[]{
                            new TestJobSubScheme{
                                Type = JobType.Defeats,
                                Value = 1
                            }
                        },
                        // Указываем минимальный подуровень.
                        // Чтобы при попытке прокачки не наткнуться на левелкап.
                        MaxValue = 1
                    }
                }
            };

            var perkJobMock = new Mock<IJob>();
            perkJobMock.SetupProperty(x => x.Progress, 1);
            perkJobMock.SetupGet(x => x.Scheme)
                .Returns(perkScheme.Levels[0].Jobs[0]);
            perkJobMock.SetupProperty(x => x.IsComplete);
            var perkJob = perkJobMock.Object;


            var progressMock = new Mock<IJobProgress>();
            progressMock.Setup(x => x.ApplyToJobs(It.IsAny<IEnumerable<IJob>>()))
                .Returns(new[] { perkJob });
            var progress = progressMock.Object;

            var perkMock = new Mock<IPerk>();
            perkMock.SetupGet(x => x.Scheme)
                .Returns(perkScheme);
            perkMock.SetupGet(x => x.CurrentLevel)
                // Перки, которые могут прокачиваться, должны обладать уровнем.
                // (0, 0) это минимальный уровень.
                .Returns(new PerkLevel(0, 0));
            var perk = perkMock.Object;

            var perks = new[] {
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

        /// <summary>
        /// Тест проверяет, что перки с максимальным уровнем не прокачиваются.
        /// </summary>
        [Test]
        public void ApplyProgress_PerkIsLevelCap_PerkDosntLeveledUp()
        {
            // ARRANGE
            var perkScheme = new PerkScheme
            {
                Levels = new[] {
                    new PerkLevelSubScheme{
                        MaxValue = 0,
                        Jobs = new IJobSubScheme[]{
                            new TestJobSubScheme{
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
            perkJobMock.SetupProperty(x => x.IsComplete);
            var perkJob = perkJobMock.Object;


            var progressMock = new Mock<IJobProgress>();
            progressMock.Setup(x => x.ApplyToJobs(It.IsAny<IEnumerable<IJob>>()))
                .Returns(new[] { perkJob });
            var progress = progressMock.Object;

            var perkMock = new Mock<IPerk>();
            perkMock.SetupGet(x => x.Scheme)
                .Returns(perkScheme);
            perkMock.SetupGet(x => x.CurrentLevel)
                .Returns(new PerkLevel(0, 0));
            var perk = perkMock.Object;

            var perks = new[] {
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
            evolutionDataMock.Verify(x => x.PerkLevelUp(It.IsAny<IPerk>()), Times.Never);
        }
    }
}