using Zilon.Core.Localization;

namespace Zilon.Core.MapGenerators
{
    public static class DiseaseNames
    {
        public static ILocalizedString[] Primary =>
            new[]
            {
                new LocalizedString { Ru = "Грипп" }, new LocalizedString { Ru = "Пневмония" },
                new LocalizedString { Ru = "Хворь" }, new LocalizedString { Ru = "Лихорадка" },
                new LocalizedString { Ru = "Болезнь" }, new LocalizedString { Ru = "Заражение" },
                new LocalizedString { Ru = "Язва" }, new LocalizedString { Ru = "Недостаточность" },
                new LocalizedString { Ru = "Инфекция" }, new LocalizedString { Ru = "Помутнение" },
                new LocalizedString { Ru = "Вирус" }
            };

        public static ILocalizedString[] PrimaryPreffix =>
            new[]
            {
                new LocalizedString { Ru = "Некро" }, new LocalizedString { Ru = "Гастро" },
                new LocalizedString { Ru = "Гипер" }, new LocalizedString { Ru = "Макро" },
                new LocalizedString { Ru = "Прото" }, new LocalizedString { Ru = "Сверх" },
                new LocalizedString { Ru = "Квази" }, new LocalizedString { Ru = "Псевдо" },
                new LocalizedString { Ru = "Черно" }, new LocalizedString { Ru = "Красно" },
                new LocalizedString { Ru = "Желто" }
            };

        public static ILocalizedString[] Secondary =>
            new[]
            {
                new LocalizedString { Ru = "Атипичный" }, new LocalizedString { Ru = "Черный" },
                new LocalizedString { Ru = "Красный" }, new LocalizedString { Ru = "Белый" },
                new LocalizedString { Ru = "Серый" }, new LocalizedString { Ru = "Желчный" },
                new LocalizedString { Ru = "Бубонный" }, new LocalizedString { Ru = "Общий" },
                new LocalizedString { Ru = "Кишечный" }, new LocalizedString { Ru = "Древний" },
                new LocalizedString { Ru = "Античный" }, new LocalizedString { Ru = "Мутировавший" }
            };

        public static ILocalizedString[] Subject =>
            new[]
            {
                new LocalizedString { Ru = "Смерти" }, new LocalizedString { Ru = "Крови" },
                new LocalizedString { Ru = "Печени" }
            };
    }
}