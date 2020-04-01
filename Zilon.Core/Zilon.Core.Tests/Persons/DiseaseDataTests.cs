using NUnit.Framework;
using Zilon.Core.Persons;
using System;
using System.Collections.Generic;
using System.Text;
using Moq;
using Zilon.Core.Diseases;
using Zilon.Core.Persons.Survival;

namespace Zilon.Core.Persons.Tests
{
    [TestFixture()]
    public class DiseaseDataTests
    {
        [Test()]
        public void Update_AllProcess_EffectAddedAndRemoved()
        {
            // ARRANGE
            var effectList = new List<IPersonEffect>();

            var effectCollectionMock = new Mock<IEffectCollection>();
            effectCollectionMock.Setup(x => x.Add(It.IsAny<IPersonEffect>())).Callback<IPersonEffect>(x => effectList.Add(x));
            effectCollectionMock.Setup(x => x.Remove(It.IsAny<IPersonEffect>())).Callback<IPersonEffect>(x => effectList.Remove(x));
            effectCollectionMock.SetupGet(x => x.Items).Returns(effectList);
            var effectCollection = effectCollectionMock.Object;

            var symptoms = new[] {
                new DiseaseSymptom{ Rule = DiseaseSymptomType.BreathDownSpeed }
            };

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
            effectCollectionMock.Verify(x => x.Add(It.Is<IPersonEffect>(x => DeaseSymptomHasRule(x))), Times.Once);
            effectCollectionMock.Verify(x => x.Remove(It.Is<IPersonEffect>(x => DeaseSymptomHasRule(x))), Times.Once);
        }

        private static bool DeaseSymptomHasRule(IPersonEffect x)
        {
            return x is DiseaseSymptomEffect diseaseSymptomEffect && diseaseSymptomEffect.Symptom.Rule == DiseaseSymptomType.BreathDownSpeed;
        }
    }
}