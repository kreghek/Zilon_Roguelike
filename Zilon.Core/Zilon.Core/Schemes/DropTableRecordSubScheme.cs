using System.Diagnostics.CodeAnalysis;

namespace Zilon.Core.Schemes
{
    /// <summary>
    /// Запись в схеме таблицы дропа.
    /// </summary>
    /// <remarks>
    /// Содержит информацию о том, какой продемет может выпасть,
    /// количество/качество и с какой вероятностью.
    /// </remarks>
    public sealed class DropTableRecordSubScheme : SubSchemeBase, IDropTableRecordSubScheme
    {
        [ExcludeFromCodeCoverage]
        public DropTableRecordSubScheme(string schemeSid, int weight)
        {
            SchemeSid = schemeSid ?? throw new System.ArgumentNullException(nameof(schemeSid));

            if (weight <= 0)
            {
                throw new System.ArgumentNullException(nameof(weight), "Вес записи в таблице дропа должен быть положительным.");
            }

            Weight = weight;
        }

        /// <summary>
        /// Схема предмета.
        /// </summary>
        public string SchemeSid { get; private set; }

        /// <summary>
        /// Вес записи в таблице дропа.
        /// </summary>
        /// <remarks>
        /// Чем выше, тем веротянее будет выбрана данная запись при разрешении дропа.
        /// </remarks>
        public int Weight { get; private set; }

        /// <summary>
        /// Минимальное количество ресурса.
        /// </summary>
        public int MinCount { get; private set; }

        /// <summary>
        /// Максимальное количество ресурса.
        /// </summary>
        public int MaxCount { get; private set; }
    }
}
