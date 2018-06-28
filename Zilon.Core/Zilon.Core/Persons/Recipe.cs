using Zilon.Core.Schemes;

namespace Zilon.Core.Persons
{
    /// <summary>
    /// Рецепт крафта.
    /// </summary>
    /// <remarks>
    /// Является предметом, который можно изучить любым персонажем,
    /// если он соответствует условиям рецепта.
    /// </remarks>
    public class Recipe : PropBase
    {

        /// <summary>
        /// Конструктор.
        /// </summary>
        /// <param name="scheme"> Схема рецепта. </param>
        public Recipe(PropScheme scheme) : base(scheme)
        {
        }
    }
}
