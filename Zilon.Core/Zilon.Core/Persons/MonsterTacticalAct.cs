using Zilon.Core.Schemes;

namespace Zilon.Core.Persons
{
    /// <summary>
    /// Тактическое действие для монстров.
    /// </summary>
    public class MonsterTacticalAct : ITacticalAct
    {
        //TODO Отказаться от зависимости от схемы для монстров.
        //Все параметры межно задавать прямо в конструкторе действия. Можно подумать над базовым действием.
        // Параметры хранить в схемах
        public TacticalActScheme Scheme { get; }

        public float MinEfficient { get; }

        public float MaxEfficient { get; }

        public MonsterTacticalAct(TacticalActScheme scheme, float multiplier)
        {
            Scheme = scheme;
            MinEfficient = CalcEfficient(scheme.Efficient.Min, multiplier);
            MaxEfficient = CalcEfficient(scheme.Efficient.Max, multiplier);
        }

        private float CalcEfficient(float baseEfficient,
            float multiplier)
        {
            return baseEfficient * multiplier;
        }
    }
}
