using System.Collections.Generic;
using System.Linq;

using Zilon.Core.CommonServices.Dices;
using Zilon.Core.Localization;
using Zilon.Core.Persons;

namespace Zilon.Core.MapGenerators
{
    public class DiseaseGenerator : IDiseaseGenerator
    {
        private readonly IDice _dice;
        private List<UsedDisease> _usedDiseases;

        public DiseaseGenerator(IDice dice)
        {
            _dice = dice;

            _usedDiseases = new List<UsedDisease>();
        }

        public IDisease Create()
        {
            var roll = _dice.RollD6();
            if (roll == 1)
            {
                var nameGenerationAttempt = 0;
                do
                {

                    var primaryName = _dice.RollFromList(DiseaseNames.Primary);

                    ILocalizedString preffix = null;

                    var rollPreffix = _dice.RollD6();
                    if (rollPreffix <= 4)
                    {
                        preffix = _dice.RollFromList(DiseaseNames.PrimaryPreffix);
                    }

                    var usedDisease = new UsedDisease() { Primary = primaryName, PrimaryPreffix = preffix };

                    // Проверяем, была ли уже такая болезнь.

                    var isDuplicate = CheckDublicate(usedDisease);
                    if (isDuplicate)
                    {
                        nameGenerationAttempt++;
                        continue;
                    }

                    var disease = new Disease(primaryName, preffix);

                    _usedDiseases.Add(usedDisease);

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

        private bool CheckDublicate(UsedDisease checkingDisease)
        {
            var foundName = _usedDiseases
                .SingleOrDefault(x => x.Primary == checkingDisease.Primary && x.PrimaryPreffix == checkingDisease.PrimaryPreffix);

            return foundName != null;
        }

        private class UsedDisease
        {
            public ILocalizedString Primary { get; set; }
            public ILocalizedString PrimaryPreffix { get; set; }
        }
    }

    public static class DiseaseNames
    {
        public static ILocalizedString[] Primary
        {
            get
            {
                return new[] {
                    new LocalizedString{
                        Ru = "Грипп"
                    },

                    new LocalizedString{
                        Ru = "Пневмония"
                    },

                    new LocalizedString{
                        Ru = "Хворь"
                    },

                    new LocalizedString{
                        Ru = "Лихорадка"
                    },

                    new LocalizedString{
                        Ru = "Болезнь"
                    },

                    new LocalizedString{
                        Ru = "Заражение"
                    },

                    new LocalizedString{
                        Ru = "Язва"
                    },

                    new LocalizedString{
                        Ru = "Недостаточность"
                    },

                    new LocalizedString{
                        Ru = "Инфекция"
                    },

                    new LocalizedString{
                        Ru = "Помутнение"
                    }
                };
            }
        }

        public static ILocalizedString[] PrimaryPreffix
        {
            get =>
                new[]{
                    new LocalizedString{
                        Ru = "Некро"
                    },
                    new LocalizedString{
                        Ru = "Гастро"
                    },
                    new LocalizedString{
                        Ru = "Гипер"
                    },
                    new LocalizedString{
                        Ru = "Макро"
                    },
                    new LocalizedString{
                        Ru = "Прото"
                    },
                    new LocalizedString{
                        Ru = "Сверх"
                    }
                };
        }
    }

    public class LocalizedString : ILocalizedString
    {
        public string En { get; set; }
        public string Ru { get; set; }
    }
}
