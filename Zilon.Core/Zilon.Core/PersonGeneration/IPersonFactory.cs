using Zilon.Core.Persons;
using Zilon.Core.Scoring;

namespace Zilon.Core.PersonGeneration
{
    public interface IPersonFactory
    {
        public IPlayerEventLogService? PlayerEventLogService { get; set; }
        IPerson Create(string personSchemeSid, IFraction fraction);
    }
}