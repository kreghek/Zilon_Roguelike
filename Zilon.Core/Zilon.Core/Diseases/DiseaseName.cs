using System;
using System.Collections.Generic;

using Zilon.Core.Localization;

namespace Zilon.Core.Diseases
{
    /// <summary>
    /// Структура для хранения имёни болезни.
    /// </summary>
    public struct DiseaseName : IEquatable<DiseaseName>
    {
        public DiseaseName(
            ILocalizedString primary,
            ILocalizedString primaryPrefix,
            ILocalizedString secondary,
            ILocalizedString subject)
        {
            Primary = primary ?? throw new ArgumentNullException(nameof(primary));
            PrimaryPrefix = primaryPrefix;
            Secondary = secondary;
            Subject = subject;
        }

        /// <summary>
        /// Основное наименование. Обяхательно есть, не null.
        /// </summary>
        public ILocalizedString Primary { get; }

        /// <summary>
        /// Префикс основного наименования. Необязательный. Слитно ставится перед основным наименованием.
        /// </summary>
        public ILocalizedString PrimaryPrefix { get; }

        /// <summary>
        /// Вторичное имя болезни. Необязательное. Ставится перед основным наименованием с префиксом.
        /// </summary>
        public ILocalizedString Secondary { get; set; }

        /// <summary>
        /// Субъект болезни. Необязательный. Ставится после основного наименования.
        /// </summary>
        public ILocalizedString Subject { get; }

        public override bool Equals(object obj)
        {
            return obj is DiseaseName name && Equals(name);
        }

        [System.Diagnostics.CodeAnalysis.SuppressMessage("StyleCop.CSharp.MaintainabilityRules", "SA1407:ArithmeticExpressionsMustDeclarePrecedence",
            Justification = "Автогенерируемый код расчёта хэша. Так принято.")]
        public override int GetHashCode()
        {
            var hashCode = -2010629649;
            hashCode = hashCode * -1521134295 + EqualityComparer<ILocalizedString>.Default.GetHashCode(Primary);
            hashCode = hashCode * -1521134295 + EqualityComparer<ILocalizedString>.Default.GetHashCode(PrimaryPrefix);
            hashCode = hashCode * -1521134295 + EqualityComparer<ILocalizedString>.Default.GetHashCode(Secondary);
            return hashCode;
        }

        public static bool operator ==(DiseaseName left, DiseaseName right)
        {
            return left.Equals(right);
        }

        public static bool operator !=(DiseaseName left, DiseaseName right)
        {
            return !(left == right);
        }

        public bool Equals(DiseaseName other)
        {
            return EqualityComparer<ILocalizedString>.Default.Equals(Primary, other.Primary) &&
                   EqualityComparer<ILocalizedString>.Default.Equals(PrimaryPrefix, other.PrimaryPrefix) &&
                   EqualityComparer<ILocalizedString>.Default.Equals(Secondary, other.Secondary);
        }
    }
}