namespace Zilon.Core.Common
{
    /// <summary>
    /// Модификаторы броска куба.
    /// </summary>
    public class RollModifiers
    {
        public RollModifiers(int resultBuff)
        {
            ResultBuff = resultBuff;
        }

        public int ResultBuff { get; }
    }
}
