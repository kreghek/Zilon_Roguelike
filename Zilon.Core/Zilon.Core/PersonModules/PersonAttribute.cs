namespace Zilon.Core.PersonModules
{
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