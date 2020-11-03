using Zilon.Core.Tactics;

namespace Zilon.Core.PersonModules
{
    public interface IFowData : IPersonModule
    {
        ISectorFowData GetSectorFowData(ISector sector);
    }
}