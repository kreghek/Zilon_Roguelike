using Zilon.Core.Components;

namespace Zilon.Core.Schemes
{
    /// <summary>
    /// Подсхема для хранения данных о крафте предмета.
    /// </summary>
    public sealed class CraftSubScheme : SubSchemeBase
    {
        /// <summary>
		/// Ресурсы (черновики), которые требуются для прототипирования.
		/// </summary>
		public PropSet[] PrototypeSources { get; set; }

        /// <summary>
        /// Инструменты, требуемые для создания предмета.
        /// </summary>
        public InstrumentLevelFunctionSubScheme[] InstrumentFunctions { get; set; }

        /// <summary>
        /// Компетенции, требуемые для создания предмета.
        /// </summary>
        public ProfessionRequirementSubScheme[] Professions { get; set; }

        /// <summary>
        /// Исходные ресурсы.
        /// </summary>
        public PropSet[] Sources { get; set; }

        /// <summary>
        /// Количество выходной продукции.
        /// </summary>
        public int ProductOutput { get; set; }

        /// <summary>
        /// Количество мастеров, тредуемых для изготовления предмета.
        /// </summary>
        public int MasterCount { get; set; }
    }
}