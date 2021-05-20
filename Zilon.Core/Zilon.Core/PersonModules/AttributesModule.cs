using System.Collections.Generic;
using System.Linq;

namespace Zilon.Core.PersonModules
{
    /// <summary>
    /// Базовая реализация модуля атрибутов.
    /// </summary>
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

        /// <inheritdoc />
        public string Key => nameof(IAttributesModule);

        /// <inheritdoc />
        public bool IsActive { get; set; }

        public PersonAttribute GetAttribute(PersonAttributeType personAttributeType)
        {
            return _attributes.SingleOrDefault(x => x.Type == personAttributeType);
        }

        /// <inheritdoc />
        public IEnumerable<PersonAttribute> GetAttributes()
        {
            return _attributes;
        }
    }
}