using System;
using System.Collections.Generic;

namespace Zilon.Core.Scoring
{
    /// <summary>
    /// Структура для хранения разобранного значения времени жизни персонажа.
    /// </summary>
    public struct DetailedLifetime : IEquatable<DetailedLifetime>
    {
        /// <summary>
        /// Конструктор объекта.
        /// </summary>
        /// <param name="days"> Количество целых прожитых дней. </param>
        /// <param name="hours"> Количество целых пожитых часов. </param>
        public DetailedLifetime(int days, int hours)
        {
            Days = days;
            Hours = hours;
        }

        /// <summary>
        /// Количество целых прожитых дней.
        /// </summary>
        public int Days { get; }

        /// <summary>
        /// Количество целых пожитых часов.
        /// </summary>
        public int Hours { get; }

        public override bool Equals(object obj)
        {
            return obj is DetailedLifetime lifetime &&
                   Days == lifetime.Days &&
                   Hours == lifetime.Hours;
        }

        public bool Equals(DetailedLifetime other)
        {
            return Days == other.Days && Hours == other.Hours;
        }

        public override int GetHashCode()
        {
            var hashCode = 1190513422;
            hashCode = (hashCode * -1521134295) + Days.GetHashCode();
            hashCode = (hashCode * -1521134295) + Hours.GetHashCode();
            return hashCode;
        }

        public static bool operator ==(DetailedLifetime left, DetailedLifetime right)
        {
            return EqualityComparer<DetailedLifetime>.Default.Equals(left, right);
        }

        public static bool operator !=(DetailedLifetime left, DetailedLifetime right)
        {
            return !(left == right);
        }
    }
}