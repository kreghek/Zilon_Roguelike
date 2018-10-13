using Zilon.Core.Schemes;

namespace Zilon.Core.Persons
{
    /// <summary>
    /// Базовый класс для всех предметов.
    /// </summary>
    public abstract class PropBase : IProp
    {
        /// <summary>
        /// Конструктор.
        /// </summary>
        /// <param name="scheme"> Схема предмета. </param>
        protected PropBase(IPropScheme scheme)
        {
            Scheme = scheme;
        }

        /// <summary>
        /// Схема предмета.
        /// </summary>
        public IPropScheme Scheme { get; }

        public override string ToString()
        {
            return $"{Scheme}";
        }
    }
}
