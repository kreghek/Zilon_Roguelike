namespace Zilon.Core.WorldGeneration.LocalityHazards
{
    /// <summary>
    /// Кризис, который возникает в городе.
    /// </summary>
    public interface ICrysis
    {
        /// <summary>
        /// Обновление состояния кризиса.
        /// </summary>
        /// <param name="locality"> Город, над которым навис кризис. </param>
        /// <returns> Возвращает true, если кризис продолжается. Иначе - false. </returns>
        bool Update(Locality locality);
    }
}
