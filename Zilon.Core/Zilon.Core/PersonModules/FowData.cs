using Zilon.Core.Tactics;

namespace Zilon.Core.PersonModules
{
    public class FowData : IFowData
    {
        private readonly IDictionary<ISector, ISectorFowData> _sectorFows;

        public FowData()
        {
            _sectorFows = new Dictionary<ISector, ISectorFowData>();
        }

        public string Key => nameof(IFowData);
        public bool IsActive { get; set; }

        public ISectorFowData GetSectorFowData(ISector sector)
        {
            if (!_sectorFows.TryGetValue(sector, out var sectorFowData))
            {
                sectorFowData = new HumanSectorFowData();
                _sectorFows[sector] = sectorFowData;
            }

            return sectorFowData;
        }
    }
}