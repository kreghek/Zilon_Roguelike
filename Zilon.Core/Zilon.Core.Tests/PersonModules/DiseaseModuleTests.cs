using System;
using System.Collections.Generic;
using Moq;
using NUnit.Framework;
using Zilon.Core.Diseases;
using Zilon.Core.PersonModules;
using Zilon.Core.Persons;
using Zilon.Core.Persons.Survival;
using Zilon.Core.Tests.Persons.TestCases;

namespace Zilon.Core.Tests.PersonModules
{
    [TestFixture]
    [Parallelizable(ParallelScope.All)]
    public class DiseaseModuleTests
    {
        [Test]
        [TestCaseSource(typeof(DiseaseDataTestCaseSource), nameof(DiseaseDataTestCaseSource.TestCases))]
        public void Update_AllProcess_EffectAddedAndRemoved(DiseaseSymptom[] symptoms)
        {
            if (symptoms is null)
            {
                throw new ArgumentNullException(nameof(symptoms));
            }

            // ARRANGE
            var effectList = new List<IPersonEffect>();

            var effectCollectionMock = new Mock<IEffectsModule>();
            effectCollectionMock.Setup(x => x.Add(It.IsAny<IPersonEffect>()))
                .Callback<IPersonEffect>(x => effectList.Add(x));
            effectCollectionMock.Setup(x => x.Remove(It.IsAny<IPersonEffect>()))
                .Callback<IPersonEffect>(x => effectList.Remove(x));
            effectCollectionMock.SetupGet(x => x.Items).Returns(effectList);
            var effectCollection = effectCollectionMock.Object;

            var diseaseMock = new Mock<IDisease>();
            diseaseMock.Setup(x => x.GetSymptoms())
                .Returns(symptoms);
            diseaseMock.SetupGet(x => x.ProgressSpeed)
                .Returns(0.001f);
            var disease = diseaseMock.Object;

            var diseaseData = new DiseaseModule();

            diseaseData.Infect(disease);

            // ACT

            for (var i = 0; i < 1000; i++)
            {
                diseaseData.Update(effectCollection);
            }

            // ARRANGE
            var exceptedTimes = symptoms.Length;
            effectCollectionMock.Verify(x => x.Add(It.Is<IPersonEffect>(effect => IsDeaseSymptom(effect))),
                Times.Exactly(exceptedTimes));
            effectCollectionMock.Verify(x => x.Remove(It.Is<IPersonEffect>(effect => IsDeaseSymptom(effect))),
                Times.Exactly(exceptedTimes));
        }

        private static bool IsDeaseSymptom(IPersonEffect x)
        {
            return x is DiseaseSymptomEffect;
        }
    }
}