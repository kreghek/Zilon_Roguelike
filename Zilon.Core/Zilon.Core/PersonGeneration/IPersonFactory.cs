using Zilon.Core.Persons;

namespace Zilon.Core.PersonGeneration
{
    public interface IPersonFactory
    {
        IPerson Create(string personSchemeSid, IFraction fraction);
    }
}