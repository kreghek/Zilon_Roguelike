using System.Collections.Generic;
using System.Linq;

namespace Zilon.Core.PersonModules
{
    /// <summary>
    /// Модуль для работы с атрибутами персонажа: сила, ловкость, интеллект.
    /// </summary>
    public interface IAttributesModule: IPersonModule
    {
        /// <summary>
        /// Возвращает перечень всех атрибутов персонажа.
        /// </summary>
        /// <returns></returns>
        IEnumerable<PersonAttribute> GetAttributes();
    }

    public sealed class AttributesModule : IAttributesModule
    {
        private readonly IEnumerable<PersonAttribute> _attributes;

        public AttributesModule(IEnumerable<PersonAttribute> attributes)
        {
            if (attributes is null)
            {
                throw new System.ArgumentNullException(nameof(attributes));
            }

            IsActive = true;

            _attributes = attributes.ToArray();
        }

        public string Key { get => nameof(IAttributesModule); }
        public bool IsActive { get; set; }

        public IEnumerable<PersonAttribute> GetAttributes()
        {
            return _attributes;
        }
    }

    /// <summary>
    /// Тип атрибута.
    /// </summary>
    public enum PersonAttributeType
    { 
        /// <summary>
        /// Физическая, грубая сила персонажа.
        /// Влияет на урон оружием блжнего боя. Выступает, как ограничение на ношение тяжелой экипировки.
        /// </summary>
        PhysicalStrength,

        /// <summary>
        /// Ловкость, физическая гибкость персонажа, растяжка.
        /// Выступает, как ограничение на экипировку, типа кинжалов, посохов.
        /// </summary>
        PhysicalAgility,

        /// <summary>
        /// Восприятие, зрение. Умение видеть, замечать.
        /// Выступает, как ограничение на использование оружие дальнего боя. Или оружия, основанного на точных ударах,
        /// а не на силе (кинжлы, копья).
        /// </summary>
        Perception
    }

    /// <summary>
    /// Объект для предоставления инфорации о состоянии атрибута персонажа.
    /// </summary>
    public sealed class PersonAttribute
    {
        public PersonAttribute(PersonAttributeType type, float value)
        {
            Type = type;
            Value = value;
        }

        public PersonAttributeType Type { get; }
        public float Value { get; }
    }
}
