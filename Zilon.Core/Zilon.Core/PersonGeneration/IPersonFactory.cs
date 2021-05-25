using Zilon.Core.Persons;
using Zilon.Core.Scoring;

namespace Zilon.Core.PersonGeneration
{
    public interface IPersonFactory
    {
        IPerson Create(string personSchemeSid, IFraction fraction);
        public IPlayerEventLogService? PlayerEventLogService { get; set; }
    }
}