using System;
using System.Drawing;
using Zilon.Core.PersonModules;

namespace Zilon.Core.Persons
{
    /// <summary>
    /// Интерфейс персонажа.
    /// </summary>
    /// <remarks>
    /// Персонаж - это описание игрового объекта за пределами тактических боёв.
    /// </remarks>
    public interface IPerson
    {
        int Id { get; set; }

        PhysicalSize PhysicalSize { get; }

        /// <summary>
        /// Получение модуля статического объекта.
        /// </summary>
        /// <typeparam name="TPersonModule">Тип модуля.</typeparam>
        /// <returns>Возвращает объект модуля.</returns>
        TPersonModule GetModule<TPersonModule>(string key) where TPersonModule : IPersonModule;

        /// <summary>
        /// Добавление модуля статического объекта.
        /// </summary>
        /// <typeparam name="TPersonModule">Тип модуля.</typeparam>
        /// <param name="sectorObjectModule">Объект модуля, который нужно добавить к объекту.</param>
        void AddModule<TPersonModule>(TPersonModule sectorObjectModule) where TPersonModule : IPersonModule;

        /// <summary>
        /// Проверка наличия модуля статического объекта.
        /// </summary>
        /// <returns>Возвращает true, если модуль указанного типа есть у объекта. Иначе, false.</returns>
        bool HasModule(string key);

        IFraction Fraction { get; }
    }

    public interface IFraction
    {
        string Name { get; }
    }

    public sealed class Fraction : IFraction
    {
        public Fraction(string name)
        {
            Name = name ?? throw new ArgumentNullException(nameof(name));
        }

        public string Name { get; }
    }

    public static class Fractions
    {
        static Fractions()
        {
            MonsterFraction = new Fraction("Monsters");
            MainPersonFraction = new Fraction("Main Hero");
        }

        public static IFraction MonsterFraction { get; private set; }

        public static IFraction MainPersonFraction { get; private set; }
    }
}