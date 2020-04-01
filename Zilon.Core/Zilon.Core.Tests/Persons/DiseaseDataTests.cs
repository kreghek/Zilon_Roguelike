using System.Collections.Generic;

using Moq;

using NUnit.Framework;

using Zilon.Core.Diseases;
using Zilon.Core.Persons.Survival;
using Zilon.Core.Tests.Persons.TestCases;

namespace Zilon.Core.Persons.Tests
{
    [TestFixture]
    [Parallelizable(ParallelScope.All)]
    public class DiseaseDataTests
    {
        [Test]
        [TestCaseSource(typeof(DiseaseDataTestCaseSource), nameof(DiseaseDataTestCaseSource.TestCases))]
        public void Update_AllProcess_EffectAddedAndRemoved(DiseaseSymptom[] symptoms)
        {
            // ARRANGE
            var effectList = new List<IPersonEffect>();

            var effectCollectionMock = new Mock<IEffectCollection>();
            effectCollectionMock.Setup(x => x.Add(It.IsAny<IPersonEffect>())).Callback<IPersonEffect>(x => effectList.Add(x));
            effectCollectionMock.Setup(x => x.Remove(It.IsAny<IPersonEffect>())).Callback<IPersonEffect>(x => effectList.Remove(x));
            effectCollectionMock.SetupGet(x => x.Items).Returns(effectList);
            var effectCollection = effectCollectionMock.Object;

            var diseaseMock = new Mock<IDisease>();
            diseaseMock.Setup(x => x.GetSymptoms())
                .Returns(symptoms);
            var disease = diseaseMock.Object;

            var diseaseData = new DiseaseData();

            diseaseData.Infect(disease);

            // ACT

            for (var i = 0; i < 200; i++)
            {
                diseaseData.Update(effectCollection);
            }

            // ARRANGE
            var exceptedTimes = symptoms.Length;
            effectCollectionMock.Verify(x => x.Add(It.Is<IPersonEffect>(x => IsDeaseSymptom(x))), Times.Exactly(exceptedTimes));
            effectCollectionMock.Verify(x => x.Remove(It.Is<IPersonEffect>(x => IsDeaseSymptom(x))), Times.Exactly(exceptedTimes));
        }

        private static bool IsDeaseSymptom(IPersonEffect x)
        {
            return x is DiseaseSymptomEffect;
        }
    }
}