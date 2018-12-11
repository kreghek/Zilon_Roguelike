using Zilon.Core.Persons;

namespace Assets.Zilon.Scripts.Services
{
    internal class HumanPersonManager : IHumanPersonManager
    {
        public HumanPerson Person { get; set; }
        public int SectorLevel { get; set; }
        public string SectorName { get; set; }
    }
}
