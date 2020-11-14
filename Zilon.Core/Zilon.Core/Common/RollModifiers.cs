using Newtonsoft.Json;

namespace Zilon.Core.Common
{
    /// <summary>
    /// Модификаторы броска куба.
    /// </summary>
    public class RollModifiers
    {
        [JsonConstructor]
        public RollModifiers(int resultBuff)
        {
            ResultBuff = resultBuff;
        }

        public int ResultBuff { get; }
    }
}