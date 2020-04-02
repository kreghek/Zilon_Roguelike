using System;
using System.Collections.Generic;
using System.Linq;

using Zilon.Core.CommonServices.Dices;
using Zilon.Core.Diseases;
using Zilon.Core.Localization;

namespace Zilon.Core.MapGenerators
{
    public class DiseaseGenerator : IDiseaseGenerator
    {
        private readonly IDice _dice;

        private readonly List<DiseaseName> _usedDiseases;

        public DiseaseGenerator(IDice dice)
        {
            _dice = dice;

            _usedDiseases = new List<DiseaseName>();
        }

        public IDisease Create()
        {
            var roll = _dice.RollD6();
            if (roll <= 6)
            {
                var nameGenerationAttempt = 0;
                do
                {
                    var primaryName = _dice.RollFromList(DiseaseNames.Primary);

                    ILocalizedString prefix = null;

                    var rollPrefix = _dice.RollD6();
                    if (rollPrefix >= 4)
                    {
                        prefix = _dice.RollFromList(DiseaseNames.PrimaryPreffix);
                    }

                    ILocalizedString secondary = null;
                    var rollSecondary = _dice.RollD6();
                    if (rollSecondary >= 4)
                    {
                        secondary = _dice.RollFromList(DiseaseNames.Secondary);
                    }

                    ILocalizedString subject = null;
                    var rollSubject = _dice.RollD6();
                    if (rollSubject >= 6)
                    {
                        subject = _dice.RollFromList(DiseaseNames.Subject);
                    }

                    var diseaseName = new DiseaseName(primaryName, prefix, secondary, subject);

                    // Проверяем, была ли уже такая болезнь.

                    var isDuplicate = CheckDuplicates(diseaseName);
                    if (isDuplicate)
                    {
                        nameGenerationAttempt++;
                        continue;
                    }

                    var rolledSymptoms = RolledSymptoms();

                    var progressSpeed = RollDiseaseProgressSpeed();

                    var disease = new Disease(diseaseName, rolledSymptoms, progressSpeed);

                    _usedDiseases.Add(diseaseName);

                    return disease;
                } while (nameGenerationAttempt < 10);

                // Не удалось сгенерировать уникальное имя. Значит вообще не генерируем болезнь.
                return null;
            }
            else
            {
                return null;
            }
        }

        private IEnumerable<DiseaseSymptom> RolledSymptoms()
        {
            var symptomCount = _dice.Roll(3, 5);

            var rolledSymptoms = _dice.RollFromList(DiseaseSymptoms.Symptoms, symptomCount);

            return rolledSymptoms;
        }

        private float RollDiseaseProgressSpeed()
        {
            const int BASE_DURABLE = 20_000;
            const float DIFF_DURABLE_PERCENTAGE = 0.1f;
            const int DIFF_DURABLE = (int)(BASE_DURABLE * DIFF_DURABLE_PERCENTAGE);

            var rolledDiff = _dice.Roll(-DIFF_DURABLE, DIFF_DURABLE);
            var factDurable = BASE_DURABLE + rolledDiff;

            return 1f / factDurable;
        }

        private bool CheckDuplicates(DiseaseName checkingDisease)
        {
            var nameFound = _usedDiseases.Any(x => x == checkingDisease);

            return nameFound;
        }
    }
}
