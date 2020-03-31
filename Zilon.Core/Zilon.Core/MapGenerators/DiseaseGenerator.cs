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

        private readonly List<DiseaseName> _usedDiseases;

        public DiseaseGenerator(IDice dice)
        {
            _dice = dice;

            _usedDiseases = new List<DiseaseName>();
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

                    var isDuplicate = CheckDublicate(diseaseName);
                    if (isDuplicate)
                    {
                        nameGenerationAttempt++;
                        continue;
                    }

                    var disease = new Disease(diseaseName);

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

        private bool CheckDublicate(DiseaseName checkingDisease)
        {
            var nameFound = _usedDiseases.Any(x => x == checkingDisease);

            return nameFound;
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

        public static ILocalizedString[] Secondary { 
            get =>new[] { 
                new LocalizedString{ 
                    Ru = "Атипичный"
                },
                new LocalizedString{
                    Ru = "Черный"
                },
                new LocalizedString{
                    Ru = "Красный"
                },
                new LocalizedString{
                    Ru = "Белый"
                },
                new LocalizedString{
                    Ru = "Желчный"
                },
                new LocalizedString{
                    Ru = "Бубонный"
                },
                new LocalizedString{ 
                    Ru = "Общий"
                },
                new LocalizedString{ 
                    Ru = "Кишечный"
                }
            };
        }

        public static ILocalizedString[] Subject {
            get => new[] {
                new LocalizedString{
                    Ru = "Смерти"
                },
                new LocalizedString{
                    Ru = "Крови"
                },
                new LocalizedString{
                    Ru = "Печени"
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
