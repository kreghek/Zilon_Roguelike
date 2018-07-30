using Zilon.Core.Components;

namespace Zilon.Core.Schemes
{
    /// <summary>
    /// Подсхема для хранения параметров зависимостей тактического действия от боевых навыков.
    /// </summary>
    public class TacticalActDependencySubScheme : SubSchemeBase
    {
        public CombatStatType Stat { get; }
        public float Value { get; }

        public TacticalActDependencySubScheme(CombatStatType stat, float value)
        {
            Stat = stat;
            Value = value;
        }
    }
}
