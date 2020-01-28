namespace Zilon.Core.World.GlobeDrafting
{
    /// <summary>
    /// Черновик мира.
    /// Содержит самую минимальную информацию.
    /// После генерации реального объекта мира черновик будет уничтожен.
    /// </summary>
    public sealed class GlobeDraft
    {
        /// <summary>
        /// Предполагаемый размер мира.
        /// </summary>
        public int Size { get; set; }

        /// <summary>
        /// Начальные поселения.
        /// Рядом с поселениями обязательно будет несколько начальных данжей.
        /// </summary>
        public RealmLocalityDraft[] StartLocalities { get; set; }
    }
}
